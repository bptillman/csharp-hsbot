namespace Hsbot.Slack.Core.Messaging
{
    public class InboundMessage
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string UserChannel { get; set; }
        public string UserEmail { get; set; }
        public string RawText { get; set; }
        public string FullText { get; set; }
        public string TargetedText { get; set; }
        public string Channel { get; set; }
        public string ChannelName { get; set; }
        public MessageRecipientType MessageRecipientType { get; set; }
        public bool BotIsMentioned { get; set; }
        public string BotName { get; set; }
        public string BotId { get; set; }
    }
}
