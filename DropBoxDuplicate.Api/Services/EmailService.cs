using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace DropBoxDuplicate.Api.Services
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            var email = new MailMessage(
                new MailAddress("digdes2017@gmail.com", "Школа разработчиков 2017"),
                new MailAddress(message.Destination))
            {
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = true
            };

            using (var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials =
                    new NetworkCredential(ConfigurationManager.AppSettings["Email:Login"],
                        ConfigurationManager.AppSettings["Email:Password"]),
                EnableSsl = true
            })
            {
                await client.SendMailAsync(email);
            }
        }
    }
}