using Shopping.Aggregator.API.Extensions;
using Shopping.Aggregator.API.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shopping.Aggregator.API.Services
{
    public class BasketService : IBasketService
    {
        private readonly HttpClient _client;

        public BasketService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<BasketModel> GetBasket(string userName)
        {
            HttpResponseMessage response = await _client.GetAsync($"/api/v1/Basket/{ userName }");

            return await response.ReadContentAs<BasketModel>();
        }
    }
}
