using Component.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.Sales.Bills
{
    public class BillHistoryVM
    {
        public int Id { get; set; }
        public string ShippedEmail { get; set; }
        public Guid UserId { get; set; }
        public string ShipName { set; get; }
        public string ShipPhoneNumber { set; get; }
        public string Address { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public string OrderCode { get; set; }

    }
}
