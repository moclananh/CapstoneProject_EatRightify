using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.Sales.Orders
{
    public class OrderDetailView
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImagePath { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

    }
}
