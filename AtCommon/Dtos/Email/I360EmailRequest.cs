using System;

namespace AtCommon.Dtos.Email
{
    public class I360EmailRequest
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string PlainTextContent { get; set; }
        public string HtmlContent { get; set; }
        public DateTime? ScheduledDateTime { get; set; }
    }
}
