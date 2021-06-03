using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;

namespace Books365WebSite.Services
{

    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; } 

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(Options.SendGridKey, subject, message, email);
        }

        public Task Execute(string apiKey, string subject, string message, string email)
        {
            #region Send email via SendGrid(commented because I don't have sendgrid free account)
            //var client = new SendGridClient(apiKey);
            //var msg = new SendGridMessage()
            //{
            //    From = new EmailAddress(Options.SendGridEmail, Options.SendGridUser),
            //    Subject = subject,
            //    PlainTextContent = message,
            //    HtmlContent = message
            //};

            //msg.AddTo(new EmailAddress(email));
            //msg.SetClickTracking(false, false);

            //return client.SendEmailAsync(msg);
            #endregion

            #region Implementation using Mikit tool
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = message;
            var sendMessage = new MimeMessage()
            {
                Subject = subject,
                Body = bodyBuilder.ToMessageBody()
                
            };
            sendMessage.From.Add(new MailboxAddress(Options.SendGridUser, Options.SendGridEmail));
            sendMessage.To.Add(new MailboxAddress(Options.SendGridUser, email));

            var client = new SmtpClient();
            
            client.Connect("smtp.gmail.com", 587, false);
            client.Authenticate(Options.SendGridEmail, Options.SendGridKey);
            return client.SendAsync(sendMessage);
            
            #endregion
        }
    }
}
