using System.Collections.Generic;

namespace Hsbot.Core.Messaging
{
    public class OutboundResponse : ResponseBase
    {
        public string Text { get; set; }
        public bool IndicateTyping { get; set; }
        
        public List<Attachment> Attachments { get; set; }
    }
}
