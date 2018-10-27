using System;
using System.Threading.Tasks;
using Hsbot.Core.Brain;

namespace Hsbot.Core.Messaging
{
    public interface IBotMessageContext
    {
        IBotBrain Brain { get; }
        IHsbotLog Log { get; }
        InboundMessage Message { get; }
        Func<OutboundResponse, Task> SendMessage { get; }
    }
}