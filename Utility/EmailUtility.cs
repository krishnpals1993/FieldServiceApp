using LaCafelogy.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using System.Net.Mail;
using System.Net;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.IO;

namespace LaCafelogy.Utility
{
    public class EmailUtility
    {
        private readonly IOptions<Appsettings> _appSettings;
        private readonly IOptions<EmailSettings> _emailSettings;
        private readonly DBContext _dbContext;

        public EmailUtility(IOptions<Appsettings> appSettings, IOptions<EmailSettings> emailSettings, DBContext dbContext)
        {
            _appSettings = appSettings;
            _emailSettings = emailSettings;
            _dbContext = dbContext;
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
                using (MailKit.Net.Smtp.SmtpClient smtpClient = new MailKit.Net.Smtp.SmtpClient())
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


        public string SendEmailWithImage(string ToEmail, string Subject, string EmailBody, string[] bccEmail, byte[] Base64)
        {
            string returnDetail = "";
            try
            {
                string toEmails = String.Join(',', bccEmail);

                MailMessage mm = new MailMessage("fieldservice.application.info@gmail.com", toEmails);
                mm.Subject = Subject;
                mm.IsBodyHtml = true;

                AlternateView alterView = ContentToAlternateView(EmailBody, Convert.ToBase64String(Base64));
                mm.AlternateViews.Add(alterView);

                var emailDetail =  _dbContext.tbl_EmailAddressDetail.FirstOrDefault();
                
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential();
                NetworkCred.UserName = emailDetail.Email;
                NetworkCred.Password = emailDetail.Password;
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                
                smtp.Send(mm);
                
                return "Email sent successfully";


            }
            catch (Exception ex)
            {
                returnDetail = ex.InnerException.Message;

            }
            return returnDetail;
        }


        private static AlternateView ContentToAlternateView(string content, string base64)
        {
            var imgCount = 0;
            List<LinkedResource> resourceCollection = new List<LinkedResource>();
            foreach (Match m in Regex.Matches(content, "<img(?<value>.*?)>"))
            {
                imgCount++;
                var imgContent = m.Groups["value"].Value;
                string type = Regex.Match(imgContent, ":(?<type>.*?);base64,").Groups["type"].Value;

                var replacement = " src=\"cid:" + imgCount + "\"";
                content = content.Replace(imgContent, replacement);
                var tempResource = new LinkedResource(Base64ToImageStream(base64))
                {
                    ContentId = imgCount.ToString()
                };
                System.Net.Mime.ContentType contentType = new System.Net.Mime.ContentType("image/jpeg");
                tempResource.ContentType = contentType;

                resourceCollection.Add(tempResource);
            }

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(content, null, MediaTypeNames.Text.Html);
            foreach (var item in resourceCollection)
            {
                alternateView.LinkedResources.Add(item);
            }

            return alternateView;
        }

        public static Stream Base64ToImageStream(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            return ms;
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
