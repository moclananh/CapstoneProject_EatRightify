using Component.Data.Entities;
using Component.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.Utilities.Comments
{
    public class CommentVm
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public int ProductId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public Status Status { get; set; }
        public int Grade { get; set; }
        public string UserAvatar { get; set; }
    }
}
