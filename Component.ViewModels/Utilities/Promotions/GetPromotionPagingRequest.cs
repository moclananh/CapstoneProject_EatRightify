using Component.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.Utilities.Promotions
{
    public class GetPromotionPagingRequest : PagingRequestBase
    {
        public string? Keyword { get; set; }
    }
}
