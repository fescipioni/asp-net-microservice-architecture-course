using AutoMapper;
using Basket.API.Entities;
using Basket.API.gRPCServices;
using Basket.API.Repositories;
using Discount.gRPC.Protos;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMapper _mapper;
        private readonly DiscountGrpcService _discountGrpcService;

        public BasketController(IBasketRepository basketRepository, IPublishEndpoint publishEndpoint, IMapper mapper, DiscountGrpcService discountGrpcService)
        {
            _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _discountGrpcService = discountGrpcService ?? throw new ArgumentNullException(nameof(discountGrpcService));
        }

        [HttpGet("{userName}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), statusCode: (int) HttpStatusCode.OK)]
        [ProducesResponseType(statusCode: (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            ShoppingCart basket = await _basketRepository.GetBasket(userName);

            return Ok(basket ?? new ShoppingCart(userName));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), statusCode: (int)HttpStatusCode.OK)]
        [ProducesResponseType(statusCode: (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            // Communicate with Discount.gRPC
            // and Calculate latest prices of product into shoppinh cart.

            CouponModel coupon = null;

            foreach (ShoppingCartItem item in basket.Items)
            {
                coupon = await _discountGrpcService.GetDiscount(item.ProductName);

                item.Price -= coupon.Amount;
            }

            return Ok(await _basketRepository.UpdateBasket(basket));
        }

        [HttpDelete("{userName}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(void), statusCode: (int)HttpStatusCode.OK)]
        [ProducesResponseType(statusCode: (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> DeleteBasket(string userName)
        {
            await _basketRepository.DeleteBasket(userName);

            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(statusCode: (int)HttpStatusCode.Accepted)]
        [ProducesResponseType(statusCode: (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(statusCode: (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            ShoppingCart basket = await _basketRepository.GetBasket(basketCheckout.UserName);

            if (basket == null)
            {
                return BadRequest("Basket was not found.");
            }

            BasketCheckoutEvent eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMessage.TotalPrice = basket.TotalPrice;

            await _publishEndpoint.Publish(eventMessage);

            await _basketRepository.DeleteBasket(basket.UserName);

            return Accepted();
        }
    }
}