using Component.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Data.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int ProductId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public Status Status { get; set; }
        public AppUser User { get; set; }
        public Product Product { get; set; }
    }
}
