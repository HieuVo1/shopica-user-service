using System;
using System.Collections.Generic;
using System.Linq;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using USER_SERVICE_NET.ViewModels.Commons;
using USER_SERVICE_NET.ViewModels.Emails;
using MimeKit;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace USER_SERVICE_NET.Services.Emails
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfiguration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public EmailService(EmailConfiguration emailConfiguration, IWebHostEnvironment webHostEnvironment)
        {
            _emailConfiguration = emailConfiguration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<APIResult<string>> SendEmailAsync(EmailRequest message)
        {
            var mailMessage = CreateEmailMessage(message);
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_emailConfiguration.SmtpServer, _emailConfiguration.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_emailConfiguration.UserName, _emailConfiguration.Password);

                    await client.SendAsync(mailMessage);
                    return new APIResultSuccess<string>();
                }
                catch (Exception e)
                {
                    return new APIResultErrors<string>(e.Message);
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }

        private MimeMessage CreateEmailMessage(EmailRequest message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Shopica Email Service",_emailConfiguration.From));
            if (message.Recipients!=null || message.Recipients.Count != 0)
            {
                foreach(var email in message.Recipients)
                {
                    emailMessage.To.Add(new MailboxAddress(email));
                }
            }
            else
            {
                emailMessage.To.Add(new MailboxAddress(message.To));
            }
            emailMessage.Subject = message.Subject;

            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message.Content };

            return emailMessage;
        }
    }
}
