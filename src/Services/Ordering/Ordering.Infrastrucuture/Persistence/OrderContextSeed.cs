using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.Infrastrucuture.Persistence
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext orderContext, ILogger<OrderContextSeed> logger)
        {
            if (!orderContext.Orders.Any())
            {
                orderContext.Orders.AddRange(GetPreconfiguredOrders());
                await orderContext.SaveChangesAsync();
                logger.LogInformation("Seed database associated with context {DbContextName}", typeof(OrderContext).Name);
            }
        }

        private static IEnumerable<Order> GetPreconfiguredOrders()
        {
            return new List<Order>
            {
                new Order() 
                {
                    UserName = "rodolpholl", 
                    FirstName = "Rodolpho", 
                    LastName = "Loreto", 
                    EmailAddress = "rodolpho.loreto@hotmail.com", 
                    AddressLine = "São Paulo", 
                    Country = "Brazil", 
                    TotalPrice = 350 
                }
            };
        }
    }
}
