namespace MQ.Models
{
    public class SMTPConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public QuotaType QuotaType { get; set; }
        public int QuotaValue { get; set; }
    }
    public enum QuotaType
    {
        Minute, Hour, Day
    }
}
