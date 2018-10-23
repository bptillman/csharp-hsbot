using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Slack.Core.Brain;

namespace Hsbot.Slack.Core.Messaging
{
    public class BotMessageContext
    {
        public BotMessageContext(IBotBrain brain, 
            IHsbotLog log, 
            InboundMessage message,
            Func<OutboundResponse, Task> sendMessageFunc)
        {
            Brain = brain;
            Log = log;
            Message = message;
            SendMessage = sendMessageFunc;
        }

        public IBotBrain Brain { get; }
        public IHsbotLog Log { get; }
        public InboundMessage Message { get; }
        public Func<OutboundResponse, Task> SendMessage { get; }
    }

    public static class BotMessageContextExtensions
    {
        /// <summary>
        /// Will generate a message to be sent the current channel the message arrived from
        /// </summary>
        public static Task ReplyToChannel(this BotMessageContext context, string text, Attachment attachment = null)
        {
            var attachments = attachment == null ? new List<Attachment>() : new List<Attachment> {attachment};
            return context.SendMessage(context.Message.ReplyToChannel(text, attachments));
        }

        /// <summary>
        /// Will generate a message to be sent the current channel the message arrived from
        /// </summary>
        public static Task ReplyToChannel(this BotMessageContext context, string text, List<Attachment> attachments)
        {
            return context.SendMessage
            (
                new OutboundResponse
                {
                    Channel = context.Message.Channel,
                    MessageRecipientType = MessageRecipientType.Channel,
                    Text = text,
                    Attachments = attachments
                }
            );
        }

        /// <summary>
        /// Will send a DirectMessage reply to the use who sent the message
        /// </summary>
        public static Task ReplyDirectlyToUser(this BotMessageContext context, InboundMessage message, string text)
        {
            return context.SendMessage
            (
                new OutboundResponse
                {
                    Channel = message.UserChannel,
                    MessageRecipientType = MessageRecipientType.DirectMessage,
                    UserId = message.UserId,
                    Text = text
                }
            );
        }

        /// <summary>
        /// Will display on Slack that the bot is typing on the current channel. Good for letting the end users know the bot is doing something.
        /// </summary>
        public static Task IndicateTypingOnChannel(this BotMessageContext context, InboundMessage message)
        {
            return context.SendMessage
            (
                new OutboundResponse
                {
                    Channel = message.Channel,
                    MessageRecipientType = MessageRecipientType.Channel,
                    Text = "",
                    IndicateTyping = true
                }
            );
        }

        /// <summary>
        /// Indicates on the DM channel that the bot is typing. Good for letting the end users know the bot is doing something.
        /// </summary>
        public static Task IndicateTypingOnDirectMessage(this BotMessageContext context, InboundMessage message)
        {
            return context.SendMessage
            (
                new OutboundResponse
                {
                    Channel = message.UserChannel,
                    MessageRecipientType = MessageRecipientType.DirectMessage,
                    UserId = message.UserId,
                    Text = "",
                    IndicateTyping = true
                }
            );
        }
    }
}