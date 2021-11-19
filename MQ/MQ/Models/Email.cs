using System.Collections.Generic;

namespace MQ.Models
{
    public class Email
    {
        public Email()
        {
            To = new List<string>();
            CC = new List<string>();
            BCC = new List<string>();
        }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> To { get; set; }
        public List<string> CC { get; set; }
        public List<string> BCC { get; set; }
        public List<Attachment> Attachments { get; set; }
    }
    public class Attachment
    {
        public string File { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}
