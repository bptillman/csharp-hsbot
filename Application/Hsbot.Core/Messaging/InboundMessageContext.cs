﻿using System.IO;
using System.Threading.Tasks;

namespace Hsbot.Core.Messaging
{
    public interface IInboundMessageContext
    {
        InboundMessage Message { get; }
        IBotMessagingServices Bot { get; }
    }

    public class InboundMessageContext : IInboundMessageContext
    {
        public InboundMessageContext(InboundMessage message, IBotMessagingServices botMessagingServices)
        {
            Message = message;
            Bot = botMessagingServices;
        }

        public InboundMessage Message { get; }
        public IBotMessagingServices Bot { get; }
    }

    public static class InboundMessageContextExtensions
    {
        public static Task<IUser> GetChatUserById(this IInboundMessageContext context, string userId)
        {
            return context.Bot.GetChatUserById(userId);
        }

        public static Task SendResponse(this IInboundMessageContext context, OutboundResponse response)
        {
            return context.Bot.SendMessage(response);
        }

        public static Task SendResponse(this IInboundMessageContext context, string text)
        {
            return context.Bot.SendMessage(context.Message.CreateResponse(text));
        }

        public static Task SendResponse(this IInboundMessageContext context, string text, Attachment attachment)
        {
            return context.Bot.SendMessage(context.Message.CreateResponse(text, attachment));
        }

        public static Task SendTypingOnChannelResponse(this IInboundMessageContext context)
        {
            return context.Bot.SendMessage(context.Message.CreateTypingOnChannelResponse());
        }

        public static Task UploadFile(this IInboundMessageContext context, Stream fileStream, string fileName)
        {
            return context.Bot.UploadFile(context.Message.CreateFileUploadResponse(fileStream, fileName));
        }
    }
}
