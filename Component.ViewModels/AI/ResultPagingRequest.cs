using Component.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.AI
{
    public class ResultPagingRequest : PagingRequestBase
    {
        public string? Keyword { get; set; }
    }
}
