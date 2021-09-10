using System.Collections.Generic;

namespace EmailRequestsRouter.Domain
{
    public class EmailMessage
    {
        public string MessageStream { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string ReplyTo { get; set; }
        public string Tag { get; set; }
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
        public string TextBody { get; set; }

        public bool? TrackOpens { get; set; }
        public LinkTrackingOptions? TrackLinks { get; set; }
        public IEnumerable<MailHeader> Headers { get; set; }
        public IDictionary<string, string> Metadata { get; set; }
        public IEnumerable<MessageAttachment> Attachments { get; set; }
    }
}
