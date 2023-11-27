using Component.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Data.Entities
{
    public class Result
    {
        public int ResultId { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public DateTime ResultDate { get; set; }
        public string Description { get; set; }
        public ResultStatus Status { get; set; }
        public bool IsSended { get; set; }
        public AppUser User { get; set; }
    }
}
