using Component.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Data.Entities
{
    public class Promotion
    {
        public int Id { set; get; }
        public string DiscountCode { set; get; }
        public DateTime FromDate { set; get; }
        public DateTime ToDate { set; get; }
        public int DiscountPercent { set; get; }
        public Status Status { set; get; }
        public string Name { set; get; }
        public string? Description { set; get; }
        public Guid? CreatedBy { get; set; }
        public AppUser User { get; set; }

    }
}
