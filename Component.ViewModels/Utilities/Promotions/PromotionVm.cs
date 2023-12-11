using Component.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.Utilities.Promotions
{
    public class PromotionVm
    {
        public int Id { set; get; }
        public Guid DiscountCode { set; get; }
        public DateTime FromDate { set; get; }
        public DateTime ToDate { set; get; }
        public int? DiscountPercent { set; get; }
        public Status Status { set; get; }
        public string Name { set; get; }
        public string? Description { set; get; }
    }
}
