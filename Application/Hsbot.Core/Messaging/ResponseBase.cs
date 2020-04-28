namespace Hsbot.Core.Messaging
{
    public abstract class ResponseBase
    {
        public string Channel { get; set; }
        public string UserId { get; set; }
        public MessageRecipientType MessageRecipientType { get; set; }
    }
}