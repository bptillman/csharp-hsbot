using System;
using System.Collections.Generic;
using System.Linq;
using SlackConnector.Models;

namespace Hsbot.Slack.Core.Messaging
{
    internal static class SlackMessageExtensions
    {
        public static string GetTextWithoutBotName(this SlackMessage incomingMessage, IEnumerable<string> botNames)
        {
            var messageText = incomingMessage.Text ?? string.Empty;
            var handle = botNames.FirstOrDefault(x => messageText.StartsWith(x, StringComparison.OrdinalIgnoreCase));

            return string.IsNullOrEmpty(handle) ? messageText : messageText.Substring(handle.Length).Trim();
        }
    }
}
