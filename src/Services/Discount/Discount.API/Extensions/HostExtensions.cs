using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            int retryForAvaiability = retry.Value;

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migrating postgresql database. ");

                    using var connection = new NpgsqlConnection
                        (configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                    connection.Open();

                    using var command = new NpgsqlCommand
                    {
                        Connection = connection
                    };

                    command.CommandText = "drop table if exists Coupon";
                    command.ExecuteNonQuery();

                    command.CommandText = @"CREATE TABLE Coupon
                                                    (id SERIAL PRIMARY KEY,
                                                     productname varchar(24) not null,
                                                     description text,
                                                     amount int)";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO coupon(productname, description, amount)	VALUES('IPhone X', 'IPhone Discount', 150)";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO coupon(productname, description, amount)	VALUES('Samsung Galaxy S10', 'Samsmung Galaxy S10 Discount', 100)";
                    command.ExecuteNonQuery();

                    logger.LogInformation("Postgresql database Migrated");

                }
                catch (Exception ex)
                {

                    logger.LogError($"An error occurs during postgresql database migration:\n {ex.Message}");
                    
                    if (retryForAvaiability < 50)
                    {
                        retryForAvaiability++;
                        System.Threading.Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, retryForAvaiability);
                    }
                }

                return host;
            }
        }
    }
}
