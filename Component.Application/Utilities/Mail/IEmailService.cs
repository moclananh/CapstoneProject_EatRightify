using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Application.Utilities.Mail
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string email, string subject, string body);
        Task SendEmailAsync(string email, string subject, string body);
    }
}
