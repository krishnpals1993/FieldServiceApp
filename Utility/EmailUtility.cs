using FieldServiceApp.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace FieldServiceApp.Utility
{
    public class EmailUtility
    {
        private readonly IOptions<Appsettings> _appSettings;
        private readonly IOptions<EmailSettings> _emailSettings;

        public EmailUtility(IOptions<Appsettings> appSettings, IOptions<EmailSettings> emailSettings)
        {
            _appSettings = appSettings;
            _emailSettings = emailSettings;
        }


        private MimeMessage CreateMimeMessageFromEmailMessage(EmailMessage message)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(message.Sender);
            foreach (var email in message.Reciever)
            {
                mimeMessage.Bcc.Add(email);
            }
            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = message.Content;
            mimeMessage.Subject = message.Subject;
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            return mimeMessage;
        }

        public string SendEmail(string ToEmail, string Subject, string EmailBody, string[] bccEmail)
        {
            string returnDetail = "";
            try
            {

                EmailMessage message = new EmailMessage();
                message.Sender = new MailboxAddress("", _emailSettings.Value.Sender);
                message.Reciever = new List<MailboxAddress>();
                foreach (var email in bccEmail)
                {
                    message.Reciever.Add(new MailboxAddress("", email));
                }


                message.Subject = Subject;
                message.Content = EmailBody;
                var mimeMessage = CreateMimeMessageFromEmailMessage(message);
                using (SmtpClient smtpClient = new SmtpClient())
                {
                    smtpClient.Connect(_emailSettings.Value.SmtpServer, _emailSettings.Value.Port, true);
                    smtpClient.Authenticate(_emailSettings.Value.UserName,_emailSettings.Value.Password);
                    smtpClient.Send(mimeMessage);
                    smtpClient.Disconnect(true);
                }
                return "Email sent successfully";


                

            }
            catch (Exception ex)
            {
                returnDetail = ex.InnerException.Message;

            }
            return returnDetail;
        }


    }

    public class EmailMessage
    {
        public MailboxAddress Sender { get; set; }
        public List<MailboxAddress> Reciever { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }

}
