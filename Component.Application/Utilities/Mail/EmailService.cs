using Component.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Component.Application.Utilities.Mail
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        public EmailService(IConfiguration configuration, UserManager<AppUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    var smtpServer = "smtp.gmail.com";
                    var smtpPort = 587;
                    var smtpUsername = "kietntce160323@fpt.edu.vn";
                    var smtpPassword = "sgap ryoq pbvs ivdz";

                    client.Host = smtpServer;
                    client.Port = smtpPort;
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(smtpUsername),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(email);

                    client.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                // Handle the exception as needed
                throw;
            }
        }

        public async Task SendPasswordResetEmailAsync(string email, string subject, string body)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Handle case where email doesn't exist
                throw new InvalidOperationException("Email not found");
            }

            try
            {
                using (var client = new SmtpClient())
                {
                    var smtpServer = "smtp.gmail.com";
                    var smtpPort = 587;
                    var smtpUsername = "kietntce160323@fpt.edu.vn";
                    var smtpPassword = "sgap ryoq pbvs ivdz";

                    client.Host = smtpServer;
                    client.Port = smtpPort;
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(smtpUsername),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(email);

                    client.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                // Handle the exception as needed
                throw;
            }
        }
    }
}
