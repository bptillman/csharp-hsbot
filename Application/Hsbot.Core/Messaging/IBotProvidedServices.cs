using System;
using System.Threading.Tasks;
using Hsbot.Core.Connection;
using Hsbot.Core.Infrastructure;
using Hsbot.Core.Messaging.Formatting;

namespace Hsbot.Core.Messaging
{
    public interface IBotProvidedServices
    {
        IHsbotLog Log { get; }
        Func<string, Task<IChatUser>> GetChatUserById { get; }
        Func<OutboundResponse, Task> SendMessage { get; }
        IChatMessageTextFormatter MessageTextFormatter { get; }
        ISystemClock SystemClock { get; }
    }
}