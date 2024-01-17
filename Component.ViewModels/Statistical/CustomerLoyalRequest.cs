using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.Statistical
{
    public class CustomerLoyalRequest
    {
        public string UserName { get; set; }
        public Guid UserId { get; set; }
        public int TotalOfOrdered { get; set; }
        public string? UserAvatar { get; set; }
    }
}
