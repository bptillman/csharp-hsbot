﻿using System;
using System.Threading.Tasks;
using Hsbot.Core.Connection;

namespace Hsbot.Core.Messaging
{
    public interface IInboundMessageContext
    {
        InboundMessage Message { get; }
        Func<OutboundResponse, Task> SendMessage { get; }
        Func<string, Task<IChatUser>> GetChatUserById { get; }
    }

    public class InboundMessageContext : IInboundMessageContext
    {
        public InboundMessageContext(InboundMessage message, Func<OutboundResponse, Task> sendMessageFunc, Func<string, Task<IChatUser>> getChatUserByIdFunc)
        {
            Message = message;
            SendMessage = sendMessageFunc;
            GetChatUserById = getChatUserByIdFunc;
        }
        
        public InboundMessage Message { get; }
        public Func<OutboundResponse, Task> SendMessage { get; }
        public Func<string, Task<IChatUser>> GetChatUserById { get; }
    }
}
