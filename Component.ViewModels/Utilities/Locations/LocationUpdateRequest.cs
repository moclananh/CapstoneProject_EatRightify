using Component.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.Utilities.Locations
{
    public class LocationUpdateRequest
    {
        public int LocationId { set; get; }
        public string LocationName { set; get; }
        public string Longitude { set; get; }
        public string Latitude { set; get; }
        public string? Description { set; get; }
        public Status Status { set; get; }
    }
}
