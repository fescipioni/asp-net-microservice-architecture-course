using Shopping.Aggregator.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shopping.Aggregator.API.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponseModel>> GetOrdersByUserName(string userName);
    }
}
