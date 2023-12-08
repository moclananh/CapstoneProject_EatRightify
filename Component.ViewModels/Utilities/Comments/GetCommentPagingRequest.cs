using Component.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.Utilities.Comments
{
    public class GetCommentPagingRequest : PagingRequestBase
    { 
        public int ProductId { get; set; }

    }
}
