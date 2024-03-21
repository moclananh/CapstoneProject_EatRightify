using Component.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.Sales.Orders
{
    public class CheckoutRequest
    {

        //Them id de tìm user
        public Guid UserId { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        public decimal TotalPriceOfOrder { set; get; }
        public int OrderMethod { get; set; }

        public List<OrderDetailVm> OrderDetails { set; get; } = new List<OrderDetailVm>();
    }
}
