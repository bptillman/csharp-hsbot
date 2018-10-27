using System;
using System.Threading.Tasks;
using Hsbot.Slack.Core.Brain;

namespace Hsbot.Slack.Core.Messaging
{
    public interface IBotMessageContext
    {
        IBotBrain Brain { get; }
        IHsbotLog Log { get; }
        InboundMessage Message { get; }
        Func<OutboundResponse, Task> SendMessage { get; }
    }
}