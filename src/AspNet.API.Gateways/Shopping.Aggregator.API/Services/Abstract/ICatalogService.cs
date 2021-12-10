using Shopping.Aggregator.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shopping.Aggregator.API.Services
{
    public interface ICatalogService
    {
        Task<IEnumerable<CatalogModel>> GetCatalog();
        Task<IEnumerable<CatalogModel>> GetCatalogByCategory(string category);
        Task<CatalogModel> GetCatalog(string id);
    }
}
