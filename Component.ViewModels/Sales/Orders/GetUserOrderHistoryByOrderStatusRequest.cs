using Component.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.Sales.Orders
{
    public class GetUserOrderHistoryByOrderStatusRequest
    {
        public Guid UserId { get; set; }
        public string? Keyword { get; set; }
        public OrderStatus? Status { get; set; }
    }
}
