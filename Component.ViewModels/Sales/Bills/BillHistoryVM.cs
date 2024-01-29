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
        public int id { get; set; }
        public string email { get; set; }
        public string userName { set; get; }
        public Guid userId { get; set; }
        public string ShipName { set; get; }
        public string ShipPhoneNumber { set; get; }
        public string address { get; set; }
        public DateTime orderDate { get; set; }
        public OrderStatus Status { get; set; }
        public Guid oderCode { get; set; }

    }
}
