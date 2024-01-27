using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.System.Users
{
    public class UpdateUserAvatarRequest
    {
        public Guid UserId { get; set; }
        public string AvatarImage {  get; set; }
    }
}
