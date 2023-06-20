using Azure;
using Azure.Communication.Email;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Respirar.Authentication.BackEnd.Application.Configuration;
using Respirar.Authentication.BackEnd.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Respirar.Authentication.BackEnd.Application.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        public MailService(IOptions<MailSettings> mailSettingsOptions)
        {
            _mailSettings = mailSettingsOptions.Value;
        }

        public async Task<bool> SendMail(MailData mailData)
        {
            try
            {
                string connectionString = _mailSettings.ConnectionString;
                EmailClient emailClient = new EmailClient(connectionString);

                var subject = mailData.EmailSubject;
                var emailContent = new EmailContent(subject)
                {
                    PlainText = mailData.EmailBody,
                    Html = $"<a href ={mailData.EmailLink}>Link de confirmacion</a>"
                };

                var sender = _mailSettings.SenderEmail;

                var emailRecipients = new EmailRecipients(new List<EmailAddress> {
                    new EmailAddress(mailData.EmailToId)
                });

                var emailMessage = new EmailMessage(sender,emailRecipients, emailContent);

                EmailSendOperation sendEmailResult = emailClient.Send(WaitUntil.Completed, emailMessage);

                return true;
            }
            catch (Exception ex)
            {
                // Exception Details
                return false;
            }
        }
    }
}
