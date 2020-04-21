using Hsbot.Core.Connection;

namespace Hsbot.Core.Tests.MessageHandler.Infrastructure
{
    public class TestChatUser : IChatUser
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public bool IsEmployee { get; set; }
    }
}
