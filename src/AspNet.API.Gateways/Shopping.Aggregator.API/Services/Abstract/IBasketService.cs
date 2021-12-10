using Shopping.Aggregator.API.Models;
using System.Threading.Tasks;

namespace Shopping.Aggregator.API.Services
{
    public interface IBasketService
    {
        Task<BasketModel> GetBasket(string userName);
    }
}
