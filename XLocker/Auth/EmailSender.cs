using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Net.Mail;
using XLocker.Emails;
using XLocker.Entities;

namespace XLocker.Auth
{
    public class EmailSender : IEmailSender<User>
    {
        private SmtpClient _smtpClient;
        private string SenderEmail = string.Empty;

        public EmailSender(IConfiguration configuration)
        {
            var credentials = new NetworkCredential(configuration["SMTP:UserName"], configuration["SMTP:Password"]);

#pragma warning disable CS8604 // Posible argumento de referencia nulo
#pragma warning disable CS8601 // Posible asignación de referencia nula
            _smtpClient = new SmtpClient
            {
                Port = int.Parse(configuration["SMTP:Port"]),
                EnableSsl = bool.Parse(configuration["SMTP:EnableSSL"]),
                Host = configuration["SMTP:Host"],
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = credentials,
            };
            SenderEmail = configuration["SMTP:Sender"];
#pragma warning restore CS8601 // Posible asignación de referencia nula
#pragma warning restore CS8604 // Posible argumento de referencia nulo
        }


        public void SendEmail(string body, string subject, string recipient)
        {
            var mail = new MailMessage(SenderEmail, recipient, subject, body);
            mail.IsBodyHtml = true;
            _smtpClient.Send(mail);
        }


        public async Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
        {
            SendEmail($"Hola {user.Name}, su codigo de confirmacion es {confirmationLink}", "Confirmacion XLocker", email);
        }

        public async Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
        {
            var emailObj = ResetPasswordEmail.BuildTemplate(user, email, resetLink);
            SendEmail(emailObj.Template, emailObj.Subject, email);
        }

        public async Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
        {
            var emailObj = ResetPasswordEmail.BuildTemplate(user, email, resetCode);
            SendEmail(emailObj.Template, emailObj.Subject, email);
        }
    }
}
