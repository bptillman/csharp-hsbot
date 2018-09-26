using SlackConnector.Models;

namespace Hsbot.Slack.Core.Messaging
{
    public static class SlackChatHubTypeExtensions
    {
        public static MessageSourceType ToMessageSourceType(this SlackChatHubType slackChatHubType)
        {
            return slackChatHubType == SlackChatHubType.DM
                ? MessageSourceType.DirectMessage
                : MessageSourceType.Channel;
        }
    }
}
