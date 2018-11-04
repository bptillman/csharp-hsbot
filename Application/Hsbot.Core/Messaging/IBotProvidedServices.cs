using System;
using System.Threading.Tasks;
using Hsbot.Core.Brain;

namespace Hsbot.Core.Messaging
{
    public interface IBotProvidedServices
    {
        IBotBrain Brain { get; }
        IHsbotLog Log { get; }
        Func<OutboundResponse, Task> SendMessage { get; }
    }
}