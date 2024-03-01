using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.System.Users
{
    public class AcceptedTermOfUseRequest
    {
        public Guid UserId { get; set; }
        public bool IsAccepted { get; set; }
    }
}
