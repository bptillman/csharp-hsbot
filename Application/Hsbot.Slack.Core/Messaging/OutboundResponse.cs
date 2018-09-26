using System.Collections.Generic;

namespace Hsbot.Slack.Core.Messaging
{
    public class OutboundResponse
    {
        public string Text { get; set; }
        public bool IndicateTyping { get; set; }
        public string Channel { get; set; }
        public string UserId { get; set; }
        public MessageSourceType MessageSourceType { get; set; }
        public List<Attachment> Attachments { get; set; }
    }
}
