using EmailSenderDemo.Data;
using EmailSenderDemo.Repository.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MQ.Models;

namespace EmailSenderDemo.Repository.Services
{
    public class EmailLogRetryRepository : IEmailLogRetryRepository
    {
        private readonly ApplicationNonSqlDbContext _dbContext;
        private readonly IMongoCollection<EmailLogRetry> _collection;
        public EmailLogRetryRepository(ApplicationNonSqlDbContext dbContext)
        {
            _dbContext = dbContext;
            _collection = _dbContext.GetCollection<EmailLogRetry>();
        }
        public async Task<List<EmailLogRetry>> AddEmailLogRetries(List<EmailLogRetry> emailLogRetries)
        {
            await _collection.InsertManyAsync(emailLogRetries);
            return emailLogRetries;
        }
        public async Task<EmailLogRetry> AddEmailLogRetry(EmailLogRetry emailLogRetries)
        {
            await _collection.InsertOneAsync(emailLogRetries);
            return emailLogRetries;
        }
        public async Task<long> CountInQuota(DateTime quotaDate, string mail)
        {
            var quotaDateFilterDef = Builders<EmailLogRetry>.Filter.Gte(r => r.CreatedDate, quotaDate);
            var mailFilterDef = Builders<EmailLogRetry>.Filter.Eq(r => r.SMTPConfigurationMail, mail);
            return await _collection.CountDocumentsAsync(quotaDateFilterDef & mailFilterDef);
        }
        public async Task<EmailLogRetry> GetByEmailLogId(string emailLogId)
        {
            var emailFilterDef = Builders<EmailLogRetry>.Filter.Eq(e => e.EmailLogId, emailLogId);
            var sortDef = Builders<EmailLogRetry>.Sort.Descending(d => d.CreatedDate);

            var options = new AggregateOptions { AllowDiskUse = true };

            return (await _collection.Aggregate(options).Match(emailFilterDef).Sort(sortDef)
                .Limit(1).ToListAsync()).FirstOrDefault();

        }
    }
}
