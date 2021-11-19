using EmailSenderDemo.Data;
using EmailSenderDemo.Repository.Interfaces;
using MongoDB.Driver;
using MQ.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmailSenderDemo.Repository.Services
{
    public class EmailLogRepository : IEmailLogRepository
    {
        private readonly ApplicationNonSqlDbContext _dbContext;
        private readonly IMongoCollection<EmailLog> _collection;
        public EmailLogRepository(ApplicationNonSqlDbContext dbContext)
        {
            _dbContext = dbContext;
            _collection = _dbContext.GetCollection<EmailLog>();
        }
        public async Task<List<EmailLog>> AddEmailLogs(List<EmailLog> emailLogs)
        {
            await _collection.InsertManyAsync(emailLogs);
            return emailLogs;
        }
        public async Task<EmailLog> AddEmailLog(EmailLog emailLog)
        {
            await _collection.InsertOneAsync(emailLog);
            return emailLog;
        }
        public async Task<List<EmailLog>> GetEmailLogs()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }
        public async Task<EmailLog> UpdateEmailLog(string Id, EmailLog emailLog)
        {
            await _collection.ReplaceOneAsync(a => a.Id == Id, emailLog);
            return emailLog;
        }
        public async Task<List<EmailLog>> GetQueuedEmailLog()
        {
            var queuedFilterDef = Builders<EmailLog>.Filter.Eq(a => a.Status, NotificationStatus.Queued);
            var options = new AggregateOptions { AllowDiskUse = true };

            return await _collection.Aggregate(options).Match(queuedFilterDef).ToListAsync();
        }
    }
}
