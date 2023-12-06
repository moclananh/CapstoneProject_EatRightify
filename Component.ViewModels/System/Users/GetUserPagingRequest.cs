using Component.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.System.Users
{
    public class GetUserPagingRequest : PagingRequestBase
    {
        public string? Keyword { get; set; }
    }
}
