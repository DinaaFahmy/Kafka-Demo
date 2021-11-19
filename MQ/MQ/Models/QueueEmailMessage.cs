using System;

namespace MQ.Models
{
    public class QueueEmailMessage
    {
        public Email Email { get; set; }
        public SMTPConfiguration SMTPConfiguration { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
