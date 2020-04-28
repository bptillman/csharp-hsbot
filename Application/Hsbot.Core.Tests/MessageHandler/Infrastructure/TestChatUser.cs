using System;

namespace Hsbot.Core.Tests.MessageHandler.Infrastructure
{
    public class TestChatUser : IUser
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public bool IsEmployee { get; set; }
        public TimeSpan TimeZoneOffset { get; }
    }
}
