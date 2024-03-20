using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.Sales.Orders
{
    public class CancelOrderRequest
    {
        public int OrderId { get; set; }
        public string? CancelDescription { get; set; }
    }
}
