using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Hsbot.Slack.Core.Messaging
{
    internal static class InboundMessageExtensions
    {
        /// <summary>
        /// Determines if message.TargetedText starts with supplied string, subject to string comparison rules (case insensitive by default)
        /// </summary>
        /// <returns>True if message.TargetedText starts with the supplied string, false otherwise</returns>
        public static bool StartsWith(this InboundMessage message, string start, StringComparison compareType = StringComparison.OrdinalIgnoreCase)
        {
            return message.TargetedText.StartsWith(start, compareType);
        }

        /// <summary>
        /// Determines if message.TargetedText contains the supplied string (case insensitive)
        /// </summary>
        /// <returns>True if message.TargetedText contains the supplied string, false otherwise</returns>
        public static bool Contains(this InboundMessage message, string text)
        {
            return message.TargetedText.ToLowerInvariant().Contains(text.ToLowerInvariant());
        }

        /// <summary>
        /// Applies regular expression to message.TargetedText to determine if message matches
        /// </summary>
        /// <returns>True if message.TargetedText is matched by regex, false otherwise</returns>
        public static bool IsMatch(this InboundMessage message, Regex matchRegex)
        {
            return matchRegex.IsMatch(message.TargetedText);
        }

        /// <summary>
        /// Applies regular expression to message.TargetedText to determine if message matches
        /// </summary>
        /// <returns>Match object with first match in message.TargetedText</returns>
        public static Match Match(this InboundMessage message, Regex matchRegex)
        {
            return matchRegex.Match(message.TargetedText);
        }

        /// <summary>
        /// Checks if a message was sent to a specific channel
        /// </summary>
        /// <returns>True if message was sent to target channel, false otherwise</returns>
        public static bool IsForChannel(this InboundMessage message, string channel)
        {
            return message.ChannelName.Equals(channel, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks if a message was sent to a specific channel
        /// </summary>
        /// <returns>True if message was sent to any of the given target channels, false otherwise</returns>
        public static bool IsForChannel(this InboundMessage message, IEnumerable<string> channels)
        {
            return channels.Any(channel => message.IsForChannel(channel));
        }

        /// <summary>
        /// Will generate a message to be sent the current channel the message arrived from
        /// </summary>
        public static OutboundResponse ReplyToChannel(this InboundMessage message, string text, Attachment attachment = null)
        {
            var attachments = attachment == null ? new List<Attachment>() : new List<Attachment> {attachment};
            return message.ReplyToChannel(text, attachments);
        }

        /// <summary>
        /// Will generate a message to be sent the current channel the message arrived from
        /// </summary>
        public static OutboundResponse ReplyToChannel(this InboundMessage message, string text, List<Attachment> attachments)
        {
            return new OutboundResponse
            {
                Channel = message.Channel,
                MessageRecipientType = MessageRecipientType.Channel,
                Text = text,
                Attachments = attachments
            };
        }

        /// <summary>
        /// Will send a DirectMessage reply to the use who sent the message
        /// </summary>
        public static OutboundResponse ReplyDirectlyToUser(this InboundMessage message, string text)
        {
            return new OutboundResponse
            {
                Channel = message.UserChannel,
                MessageRecipientType = MessageRecipientType.DirectMessage,
                UserId = message.UserId,
                Text = text
            };
        }

        /// <summary>
        /// Will display on Slack that the bot is typing on the current channel. Good for letting the end users know the bot is doing something.
        /// </summary>
        public static OutboundResponse IndicateTypingOnChannel(this InboundMessage message)
        {
            return new OutboundResponse
            {
                Channel = message.Channel,
                MessageRecipientType = MessageRecipientType.Channel,
                Text = "",
                IndicateTyping = true
            };
        }

        /// <summary>
        /// Indicates on the DM channel that the bot is typing. Good for letting the end users know the bot is doing something.
        /// </summary>
        public static OutboundResponse IndicateTypingOnDirectMessage(this InboundMessage message)
        {
            return new OutboundResponse
            {
                Channel = message.UserChannel,
                MessageRecipientType = MessageRecipientType.DirectMessage,
                UserId = message.UserId,
                Text = "",
                IndicateTyping = true
            };
        }
    }
}
