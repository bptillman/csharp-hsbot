using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Hsbot.Core.Messaging
{
    internal static class InboundMessageExtensions
    {
        /// <summary>
        /// Determines if message.TargetedText starts with supplied string, subject to string comparison rules (case insensitive by default)
        /// </summary>
        /// <returns>True if message.TargetedText starts with the supplied string, false otherwise</returns>
        public static bool StartsWith(this InboundMessage message, string start, StringComparison compareType = StringComparison.OrdinalIgnoreCase)
        {
            return message.TextWithoutBotName.StartsWith(start, compareType);
        }

        /// <summary>
        /// Determines if message.TargetedText ends with supplied string, subject to string comparison rules (case insensitive by default)
        /// </summary>
        /// <returns>True if message.TargetedText ends with the supplied string, false otherwise</returns>
        public static bool EndsWith(this InboundMessage message, string end, StringComparison compareType = StringComparison.OrdinalIgnoreCase)
        {
            return message.TextWithoutBotName.EndsWith(end, compareType);
        }

        /// <summary>
        /// Determines if message.TargetedText contains the supplied string (case insensitive)
        /// </summary>
        /// <returns>True if message.TargetedText contains the supplied string, false otherwise</returns>
        public static bool Contains(this InboundMessage message, string text)
        {
            return message.TextWithoutBotName.ToLowerInvariant().Contains(text.ToLowerInvariant());
        }

        /// <summary>
        /// Applies regular expression to message.TargetedText to determine if message matches
        /// </summary>
        /// <returns>True if message.TargetedText is matched by regex, false otherwise</returns>
        public static bool IsMatch(this InboundMessage message, Regex matchRegex)
        {
            return matchRegex.IsMatch(message.TextWithoutBotName);
        }

        /// <summary>
        /// Applies regular expression to message.TargetedText to determine if message matches
        /// </summary>
        /// <returns>Match object with first match in message.TargetedText</returns>
        public static Match Match(this InboundMessage message, Regex matchRegex)
        {
            return matchRegex.Match(message.TextWithoutBotName);
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
        public static OutboundResponse CreateResponse(this InboundMessage message, string text, Attachment attachment = null)
        {
            var attachments = attachment == null ? new List<Attachment>() : new List<Attachment> {attachment};
            return message.CreateResponse(text, attachments);
        }

        /// <summary>
        /// Will generate a message to be sent the current channel the message arrived from
        /// </summary>
        public static OutboundResponse CreateResponse(this InboundMessage message, string text, List<Attachment> attachments)
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
        /// Will display on Slack that the bot is typing on the current channel. Good for letting the end users know the bot is doing something.
        /// </summary>
        public static OutboundResponse CreateTypingOnChannelResponse(this InboundMessage message)
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
        /// Will upload a file and push a response to the channel with a link to the file
        /// </summary>
        public static FileUploadResponse CreateFileUploadResponse(this InboundMessage message, Stream fileStream, string fileName)
        {
            return new FileUploadResponse
            {
                Channel = message.Channel,
                FileName = fileName,
                FileStream = fileStream,
                MessageRecipientType = MessageRecipientType.Channel
            };
        }
    }
}
