using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.ViewModels.System.Users
{
    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public string VerifyCode { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
