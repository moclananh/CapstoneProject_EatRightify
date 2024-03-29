using Component.Data.Enums;
using Component.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.Sales.Orders
{
    public class CheckOrderResult<T> 
    {
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public string? CancelDescription { set; get; }
        public string? RefundDescription { set; get; }
        public decimal? TotalPrice { get; set; }
        public List<T> Items { set; get; }
    }
}
