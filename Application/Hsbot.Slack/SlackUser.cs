using Hsbot.Core;

namespace Hsbot.Slack
{
    public class SlackUser : IUser
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public bool IsEmployee { get; set; }
    }
}
