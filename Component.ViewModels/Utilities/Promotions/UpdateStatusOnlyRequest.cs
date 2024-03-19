using Component.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.Utilities.Promotions
{
    public class UpdateStatusOnlyRequest
    {
        public int PromotionId { get; set; }
        public Status Status { get; set; }
    }
}
