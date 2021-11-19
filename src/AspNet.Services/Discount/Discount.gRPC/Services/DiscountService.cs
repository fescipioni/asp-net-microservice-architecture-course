using AutoMapper;
using Discount.gRPC.Entities;
using Discount.gRPC.Protos;
using Discount.gRPC.Repositories;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.gRPC.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DiscountService> _logger;

        public DiscountService(IDiscountRepository discountRepository, IMapper mapper, ILogger<DiscountService> logger)
        {
            _discountRepository = discountRepository ?? throw new ArgumentNullException(nameof(discountRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            Coupon coupon = await _discountRepository.GetDiscount(request.ProductName);

            if (coupon == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ProductName={ request.ProductName } is not found."));
            }

            _logger.LogInformation($"Discount is retrieved for ProductName: { coupon.ProductName }, Description: { coupon.Description }, Amount: { coupon.Amount }");

            CouponModel couponModel = _mapper.Map<CouponModel>(coupon);

            return couponModel;
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            Coupon coupon = _mapper.Map<Coupon>(request.Coupon);

            await _discountRepository.CreateDiscount(coupon);
            
            _logger.LogInformation("Discount is successfully created. ProductName : {ProductName}", coupon.ProductName);

            CouponModel couponModel = _mapper.Map<CouponModel>(coupon);
            
            return couponModel;
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            Coupon coupon = _mapper.Map<Coupon>(request.Coupon);

            await _discountRepository.UpdateDiscount(coupon);
            
            _logger.LogInformation("Discount is successfully updated. ProductName : {ProductName}", coupon.ProductName);

            CouponModel couponModel = _mapper.Map<CouponModel>(coupon);
            
            return couponModel;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            bool deleted = await _discountRepository.DeleteDiscount(request.ProductName);

            DeleteDiscountResponse response = new DeleteDiscountResponse
            {
                Success = deleted
            };

            return response;
        }
    }
}
