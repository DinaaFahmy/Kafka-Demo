using MQ.Models;
using EmailSenderDemo.Repository.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Hosting;
using MimeKit;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EmailSenderDemo.MQ
{
    public class EmailService : BackgroundService
    {
        private readonly IEmailLogRepository _emailLogRepository;
        private readonly IEmailLogRetryRepository _emailLogRetryRepository;
        public EmailService(IEmailLogRepository emailLogRepository, IEmailLogRetryRepository emailLogRetryRepository)
        {
            _emailLogRepository = emailLogRepository;
            _emailLogRetryRepository = emailLogRetryRepository;
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var queuedList = await _emailLogRepository.GetQueuedEmailLog();

                foreach (var queued in queuedList)
                {
                    //check smtp within quota
                    //if yes, send emails
                    var quotaDate = queued.SMTPConfiguration.QuotaType
                        switch
                    {
                        QuotaType.Minute => DateTime.UtcNow.AddMinutes(-1),
                        QuotaType.Hour => DateTime.UtcNow.AddHours(-1),
                        QuotaType.Day => DateTime.UtcNow.AddDays(-1),
                        _ => throw new NotImplementedException(),
                    };

                    var inQuotaCount = await _emailLogRetryRepository.CountInQuota(quotaDate, queued.SMTPConfiguration.Mail);
                    var filteredEmailRetry = await _emailLogRetryRepository.GetByEmailLogId(queued.Id);

                    if (inQuotaCount < queued.SMTPConfiguration.QuotaValue &&
                        queued.CurrentRetriesCount < 3 /*MaximimRetriesCount*/ &&
                        (filteredEmailRetry == null || (DateTime.UtcNow - filteredEmailRetry.CreatedDate).TotalSeconds < 10000 /*TimeBetweenRetries*/))
                    {
                        await TrySendEmail(queued);
                        break;
                    }
                }
                if (queuedList.Count() == 0)
                {
                    await Task.Delay(1000, stoppingToken);
                   Console.WriteLine("No emails in the queue");
                }
            }
    }
        public async Task SendEmail(EmailLog message)
        {
            MimeMessage msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(message.SMTPConfiguration.DisplayName, message.SMTPConfiguration.Mail));
            foreach (var emailAddress in message.Email.To)
            {
                msg.To.Add(new MailboxAddress(message.SMTPConfiguration.DisplayName, emailAddress));
            }
            if (message.Email.CC?.Count > 0)
            {
                foreach (var emailAddress in message.Email.CC)
                {
                    msg.Cc.Add(new MailboxAddress(message.SMTPConfiguration.DisplayName, emailAddress));
                }
            }
            if (message.Email.BCC?.Count > 0)
            {
                foreach (var emailAddress in message.Email.BCC)
                {
                    msg.Bcc.Add(new MailboxAddress(message.SMTPConfiguration.DisplayName, emailAddress));
                }
            }

            msg.Subject = message.Email.Subject;
            var bodyBuilder = new BodyBuilder { HtmlBody = message.Email.Body };

            if (message.Email.Attachments != null && message.Email.Attachments.Any())
            {
                foreach (var attachment in message.Email.Attachments)
                {
                    byte[] fileBytes = Convert.FromBase64String(attachment.File);
                    using (var ms = new MemoryStream(fileBytes))
                    {
                        fileBytes = ms.ToArray();
                    }
                    bodyBuilder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
                }
            }
            msg.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(message.SMTPConfiguration.Host, message.SMTPConfiguration.Port, SecureSocketOptions.StartTls);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(message.SMTPConfiguration.Mail, message.SMTPConfiguration.Password);
                await client.SendAsync(msg);
                await client.DisconnectAsync(true);
            };
        }
        public async Task TrySendEmail(EmailLog emailLog)
        {
            try
            {
                await SendEmail(emailLog);
            }
            catch (Exception ex)
            {
                var emailLogRetryFailed = new EmailLogRetry
                {
                    CreatedDate = DateTime.UtcNow,
                    ErrorMessage = ex.Message.ToString(),
                    EmailLogId = emailLog.Id,
                    Succeeded = false,
                    SMTPConfigurationMail = emailLog.SMTPConfiguration.Mail
                };
                await _emailLogRetryRepository.AddEmailLogRetry(emailLogRetryFailed);
                emailLog.CurrentRetriesCount++;

                if (emailLog.CurrentRetriesCount >= 3 /*MaximumRetriesCount*/)
                    emailLog.Status = NotificationStatus.Failed;
                await _emailLogRepository.UpdateEmailLog(emailLog.Id, emailLog);
                return;
            }
            var emailLogRetrySucceeded = new EmailLogRetry
            {
                CreatedDate = DateTime.UtcNow,
                EmailLogId = emailLog.Id,
                Succeeded = true,
                SMTPConfigurationMail = emailLog.SMTPConfiguration.Mail
            };
            await _emailLogRetryRepository.AddEmailLogRetry(emailLogRetrySucceeded);
            emailLog.Status = NotificationStatus.Sent;
            await _emailLogRepository.UpdateEmailLog(emailLog.Id, emailLog);
        }
    }
}
