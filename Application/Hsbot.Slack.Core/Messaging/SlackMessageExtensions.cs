using System;
using System.Collections.Generic;
using System.Linq;
using SlackConnector.Models;

namespace Hsbot.Slack.Core.Messaging
{
    internal static class SlackMessageExtensions
    {
        public static string GetTargetedText(this SlackMessage incomingMessage, IEnumerable<string> botNames)
        {
            var messageText = incomingMessage.Text ?? string.Empty;
            var isOnPrivateChannel = incomingMessage.ChatHub.Type == SlackChatHubType.DM;

            var handle = botNames.FirstOrDefault(x => messageText.StartsWith(x, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(handle) && !isOnPrivateChannel)
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(handle) && isOnPrivateChannel)
            {
                return messageText;
            }

            return messageText.Substring(handle.Length).Trim();
        }
    }
}
