using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Threading;

namespace Discount.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            int retryForAvailability = retry.Value;

            using (IServiceScope scope = host.Services.CreateScope())
            {
                IServiceProvider services = scope.ServiceProvider;

                IConfiguration configuration = services.GetRequiredService<IConfiguration>();

                ILogger logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migrating postgresql database.");

                    using (NpgsqlConnection connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
                    {
                        connection.Open();

                        using (var command = new NpgsqlCommand{ Connection = connection })
                        {
                            command.CommandText = "DROP TABLE IF EXISTS Coupon";
                            
                            command.ExecuteNonQuery();

                            command.CommandText = @"CREATE TABLE Coupon(
                                                                          Id SERIAL PRIMARY KEY,
                                                                          ProductName VARCHAR(24) NOT NULL,
                                                                          Description TEXT,
                                                                          Amount INT
                                                                       )";

                            command.ExecuteNonQuery();

                            command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('IPhone X', 'IPhone Discount', 150);";
                            
                            command.ExecuteNonQuery();

                            command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Samsung 10', 'Samsung Discount', 100);";
                            
                            command.ExecuteNonQuery();
                        };
                    };

                    logger.LogInformation("Migrated postresql database.");
                }
                catch (NpgsqlException exception)
                {
                    logger.LogError(exception, "An error occurred while migrating the postresql database.");

                    if (retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, retryForAvailability);
                    }
                }

                return host;
            }
        }
    }
}
