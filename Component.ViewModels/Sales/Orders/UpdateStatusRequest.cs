using Component.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.Sales.Orders
{
    public class UpdateStatusRequest
    {
        public int OrderId { get; set; }
        public OrderStatus Status { set; get; }
    }
}
