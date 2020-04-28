using System;
using System.Threading.Tasks;
using Hsbot.Core.Connection;

namespace Hsbot.Core.Messaging
{
    public interface IInboundMessageContext
    {
        InboundMessage Message { get; }
        Func<OutboundResponse, Task> SendMessage { get; }
        Func<string, Task<IUser>> GetChatUserById { get; }
    }

    public class InboundMessageContext : IInboundMessageContext
    {
        public InboundMessageContext(InboundMessage message, Func<OutboundResponse, Task> sendMessageFunc, Func<string, Task<IUser>> getChatUserByIdFunc)
        {
            Message = message;
            SendMessage = sendMessageFunc;
            GetChatUserById = getChatUserByIdFunc;
        }
        
        public InboundMessage Message { get; }
        public Func<OutboundResponse, Task> SendMessage { get; }
        public Func<string, Task<IUser>> GetChatUserById { get; }
    }

    public static class InboundMessageContextExtensions
    {
        public static Task SendResponse(this IInboundMessageContext context, string text)
        {
            return context.SendMessage(context.Message.CreateResponse(text));
        }

        public static Task SendTypingOnChannelResponse(this IInboundMessageContext context)
        {
            return context.SendMessage(context.Message.CreateTypingOnChannelResponse());
        }
    }
}
