using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using XLocker.Data;
using XLocker.DTOs.Common;
using XLocker.Emails;
using XLocker.Entities;

namespace XLocker.Services
{
    public interface IEmailService
    {
        Task<bool> Deposit(Service service);

        Task<bool> Reminder(Service service);

        Task<bool> UrgentReminder(Service service);

        Task<bool> DueService(Service service);

        Task<bool> Withdrawl(Service service);

        Task<bool> AccountCreated(User user);

        Task<bool> SubmitInfoForm(InfoFormDTO info);

        void SendEmail(string from, string subject, string body);
    }
    public class EmailService : IEmailService
    {
        private readonly DataContext _context;
        private SmtpClient _smtpClient;
        private string SenderEmail = string.Empty;
        private string ReceiverEmail = string.Empty;

        public EmailService(DataContext context, IConfiguration configuration)
        {
            _context = context;
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
            ReceiverEmail = configuration["SMTP:Receiver"];
#pragma warning restore CS8601 // Posible asignación de referencia nula
#pragma warning restore CS8604 // Posible argumento de referencia nulo
        }

        public void SendEmail(string from, string subject, string body)
        {
            var mail = new MailMessage(SenderEmail, from, subject, body);
            mail.IsBodyHtml = true;
            _smtpClient.Send(mail);
        }


        public async Task<bool> AccountCreated(User user)
        {
            if (!string.IsNullOrEmpty(user.Email))
            {
                var template = await _context.EmailTemplates.Where(x => x.Name == AccountCreatedEmail.Name).FirstOrDefaultAsync();

                var emailDef = AccountCreatedEmail.BuildTemplate(user, user.Email, template);

                SendEmail(user.Email, emailDef.Subject, emailDef.Template);
                return true;
            }
            return false;
        }

        public async Task<bool> SubmitInfoForm(InfoFormDTO info)
        {
            if (!string.IsNullOrEmpty(ReceiverEmail))
            {
                var template = await _context.EmailTemplates.Where(x => x.Name == InfoFormEmail.Name).FirstOrDefaultAsync();
                var emailDef = InfoFormEmail.BuildTemplate(info, template);

                SendEmail(ReceiverEmail, emailDef.Subject, emailDef.Template);
                return true;
            }
            return false;
        }


        public async Task<bool> Deposit(Service service)
        {
            if (!string.IsNullOrEmpty(service.User.Email))
            {
                var template = await _context.EmailTemplates.Where(x => x.Name == DepositEmail.Name).FirstOrDefaultAsync();

                var emailDef = DepositEmail.BuildTemplate(service, template);

                SendEmail(service.User.Email, emailDef.Subject, emailDef.Template);
                return true;
            }
            return false;
        }

        public async Task<bool> Reminder(Service service)
        {
            if (!string.IsNullOrEmpty(service.User.Email))
            {
                var template = await _context.EmailTemplates.Where(x => x.Name == ReminderEmail.Name).FirstOrDefaultAsync();

                var emailDef = ReminderEmail.BuildTemplate(service, template);

                SendEmail(service.User.Email, emailDef.Subject, emailDef.Template);
                return true;
            }
            return false;
        }

        public async Task<bool> UrgentReminder(Service service)
        {
            if (!string.IsNullOrEmpty(service.User.Email))
            {
                var template = await _context.EmailTemplates.Where(x => x.Name == UrgentReminderEmail.Name).FirstOrDefaultAsync();

                var emailDef = UrgentReminderEmail.BuildTemplate(service, template);

                SendEmail(service.User.Email, emailDef.Subject, emailDef.Template);
                return true;
            }
            return false;
        }

        public async Task<bool> DueService(Service service)
        {
            if (!string.IsNullOrEmpty(service.User.Email))
            {
                var template = await _context.EmailTemplates.Where(x => x.Name == DueServiceEmail.Name).FirstOrDefaultAsync();

                var emailDef = DueServiceEmail.BuildTemplate(service, template);

                SendEmail(service.User.Email, emailDef.Subject, emailDef.Template);
                return true;
            }
            return false;
        }

        public async Task<bool> Withdrawl(Service service)
        {
            if (!string.IsNullOrEmpty(service.User.Email))
            {
                var template = await _context.EmailTemplates.Where(x => x.Name == WithdrawlEmail.Name).FirstOrDefaultAsync();

                var emailDef = WithdrawlEmail.BuildTemplate(service, template);

                SendEmail(service.User.Email, emailDef.Subject, emailDef.Template);
                return true;
            }
            return false;
        }


    }
}
