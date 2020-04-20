using Hsbot.Core.Connection;

namespace Hsbot.Slack
{
    public class SlackUser : IChatUser
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string FullName { get; set; }
    }
}
