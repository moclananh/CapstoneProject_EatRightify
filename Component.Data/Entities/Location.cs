using Component.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Data.Entities
{
    public class Location
    {
        public int LocationId { set; get; }
        public string LocationName { set; get; }
        public string Longitude { set; get; }
        public string Latitude { set; get; }
        public string? Description { set; get; }
        public Status Status { set; get; }
        public DateTime? DateCreated { set; get; }
        public Guid? CreatedBy { get; set; }
        public AppUser User { get; set; }
    }
}
