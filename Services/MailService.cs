using Microsoft.Extensions.Options;
using PhotographyPortfolio.Models;
using System.Net;
using System.Net.Mail;

namespace PhotographyPortfolio.Services
{
    public class MailService
    {
        private readonly MailSettings _settings;

        public MailService(IOptions<MailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(string subject, string body, string fromEmail)
        {
            using (var smtp = new SmtpClient(_settings.Host, _settings.Port))
            {
                smtp.Credentials = new NetworkCredential(_settings.Mail, _settings.Password);
                smtp.EnableSsl = true;

                var mail = new MailMessage
                {
                    From = new MailAddress(_settings.Mail, _settings.DisplayName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mail.To.Add(_settings.Mail); // Send to yourself (admin)
                mail.ReplyToList.Add(new MailAddress(fromEmail)); // allow reply to sender

                await smtp.SendMailAsync(mail);
            }
        }
    }
}
