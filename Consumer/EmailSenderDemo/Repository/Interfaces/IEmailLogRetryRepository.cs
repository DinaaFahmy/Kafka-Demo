using MQ.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailSenderDemo.Repository.Interfaces
{
    public interface IEmailLogRetryRepository
    {
        Task<List<EmailLogRetry>> AddEmailLogRetries(List<EmailLogRetry> emailLogRetries);
        Task<EmailLogRetry> AddEmailLogRetry(EmailLogRetry emailLogRetries);
        Task<long> CountInQuota(DateTime quotaDate, string mail);
        Task<EmailLogRetry> GetByEmailLogId(string emailLogId);
    }
}
