using Hsbot.Core.Messaging;
using SlackConnector.Models;

namespace Hsbot.Slack
{
    public static class SlackChatHubTypeExtensions
    {
        public static MessageRecipientType ToMessageRecipientType(this SlackChatHubType slackChatHubType)
        {
            return slackChatHubType == SlackChatHubType.DM
                ? MessageRecipientType.DirectMessage
                : MessageRecipientType.Channel;
        }
    }
}
