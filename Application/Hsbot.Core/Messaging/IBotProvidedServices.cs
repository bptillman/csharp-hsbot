using System;
using System.Threading.Tasks;
using Hsbot.Core.Connection;

namespace Hsbot.Core.Messaging
{
    [Obsolete]
    public interface IBotProvidedServices
    {
        Func<string, Task<IChatUser>> GetChatUserById { get; }
        Func<OutboundResponse, Task> SendMessage { get; }
    }
}