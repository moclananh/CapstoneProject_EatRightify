using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.System.Users
{
    public class UpdatePasswordRequest
    {
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
    }
}
