using EmailSenderDemo.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using MQ.MQServices;
using MQ.Models;

namespace EmailSenderDemo.MQ
{
    public class MQ_Service : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly MQService _mqService;
        public MQ_Service(IServiceScopeFactory serviceScopeFactory, MQService mqService)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _mqService = mqService;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                await _mqService.ConsumeEmailMessage(async message =>
                {
                    var scope = _serviceScopeFactory.CreateScope();
                    var notificationEmailLogService = scope.ServiceProvider.GetService<IEmailLogRepository>();

                    EmailLog emailLog = new EmailLog()
                    {
                        QueuedDate = DateTime.UtcNow,
                        CurrentRetriesCount = 0,
                        Email = message.Email,
                        CreatedDate = message.CreatedDate,
                        Status = NotificationStatus.Queued,
                        SMTPConfiguration = message.SMTPConfiguration
                    };
                    await notificationEmailLogService.AddEmailLog(emailLog);
                });
            });
        }
    }
}
