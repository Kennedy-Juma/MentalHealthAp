using System.Net.Mail;
using System.Net;
using MentalHealthAp.Models;

namespace MentalHealthAp.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;

        public EmailService(MailSettings mailSettings)
        {
            _mailSettings = mailSettings;
        }

        public bool SendEmail(Email request)
        {
            try
            {

                var smtpClient = new SmtpClient(_mailSettings.Server)
                {
                    Port = Int32.Parse(_mailSettings.Port),
                    Credentials = new NetworkCredential(_mailSettings.UserName, _mailSettings.Password),
                    EnableSsl = true,
                };

                var message = new MailMessage
                {
                    From = new MailAddress(_mailSettings.SenderEmail),
                    Subject = request.Subject,
                    Body = request.Body,
                    Priority = MailPriority.High,
                    IsBodyHtml = true,

                };
                message.To.Add(request.To);

                smtpClient.Send(message);
                return true;
            }

            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
