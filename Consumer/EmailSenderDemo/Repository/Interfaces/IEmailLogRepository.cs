using MQ.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailSenderDemo.Repository.Interfaces
{
    public interface IEmailLogRepository
    {
        Task<List<EmailLog>> AddEmailLogs(List<EmailLog> emailLogs);
        Task<EmailLog> AddEmailLog(EmailLog emailLog);
        Task<List<EmailLog>> GetEmailLogs();
        Task<EmailLog> UpdateEmailLog(string Id, EmailLog emailLog);
        Task<List<EmailLog>> GetQueuedEmailLog();
    }
}
