using Component.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.Utilities.Comments
{
    public class CommentCreateRequest
    {
        public Guid UserId { get; set; }
        public int ProductId { get; set; }
        public string Content { get; set; }
    }
}
