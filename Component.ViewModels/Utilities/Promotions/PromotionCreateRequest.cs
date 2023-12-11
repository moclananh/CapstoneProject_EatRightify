using Component.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.Utilities.Promotions
{
    public class PromotionCreateRequest
    {
        [Required]
        public DateTime FromDate { set; get; }
        [Required]
        public DateTime ToDate { set; get; }
        [Range(0, 100, ErrorMessage = "Range must be between 0 and 100")]
        public int? DiscountPercent { set; get; }
        [Required(ErrorMessage = "Please enter name")]
        public string Name { set; get; }
        public string? Description { set; get; }
    }
}
