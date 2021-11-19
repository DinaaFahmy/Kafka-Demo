using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MQ.Models
{
    public class EmailLogRetry
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string EmailLogId { get; set; }
        public bool Succeeded { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime CreatedDate { get; set; }
        public string SMTPConfigurationMail { get; set; }

    }
}
