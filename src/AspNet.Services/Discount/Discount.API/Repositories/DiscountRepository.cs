using Dapper;
using Discount.API.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.API.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _configuration;

        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            Coupon coupon = null;

            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
            {
                coupon = await connection.QueryFirstOrDefaultAsync<Coupon>("SELECT * FROM Coupon WHERE ProductName = @ProductName", new { ProductName = productName });

                if (coupon == null)
                {
                    coupon = new Coupon
                    {
                        ProductName = "No Discount",
                        Amount = 0,
                        Description = "No Discount Desc"
                    };
                }
            }

            return coupon;
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            bool hasSucceeded = false;

            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
            {
                int affected = await connection.ExecuteAsync("INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)", new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });

                hasSucceeded = (affected != 0);
            }

            return hasSucceeded;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            bool hasSucceeded = false;

            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
            {
                int affected = await connection.ExecuteAsync("UPDATE Coupon SET ProductName = @ProductName, Description = @Description, Amount = @Amount WHERE Id = @Id", new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount, Id = coupon.Id });

                hasSucceeded = (affected != 0);
            }

            return hasSucceeded;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            bool hasSucceeded = false;

            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
            {
                int affected = await connection.ExecuteAsync("DELETE FROM Coupon WHERE ProductName = @ProductName", new { ProductName = productName });

                hasSucceeded = (affected != 0);
            }

            return hasSucceeded;
        }
    }
}
