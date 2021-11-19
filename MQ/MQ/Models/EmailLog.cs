using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MQ.Models
{
    public class EmailLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public Email Email { get; set; }
        public SMTPConfiguration SMTPConfiguration { get; set; }
        public NotificationStatus Status { get; set; }
        public int CurrentRetriesCount { get; set; }
        public DateTime QueuedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public enum NotificationStatus
    {
        Queued, Sent, Failed
    }
}
