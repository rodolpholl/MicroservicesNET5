using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Entities
{
    public class ShoppingCartItem
    {
        public int Quantity { get; set; }
        public string Color { get; set; }
        public decimal UnitPrice { get; set; }  
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal DiscountValue { get; set; }

        public decimal TotalPrice { 
            get
            {
                return Quantity * (UnitPrice - DiscountValue);
            }
        }
    }
}
