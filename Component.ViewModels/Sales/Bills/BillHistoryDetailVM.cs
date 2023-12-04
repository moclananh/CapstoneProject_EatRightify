using Component.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.Sales.Bills
{
    public class BillHistoryDetailVM
    {
        public int ID { get; set; }
        public List<string> name { get; set; }
        public List<int> quantity { get; set; }
        public List<decimal> price { get; set; }
        public OrderStatus status { get; set; }
    }
}
