using System;
using System.Threading.Tasks;
using Hsbot.Core.ApiClients;
using Hsbot.Core.Brain;
using Hsbot.Core.Infrastructure;
using Hsbot.Core.Messaging.Formatting;

namespace Hsbot.Core.Messaging
{
    public interface IBotProvidedServices
    {
        IBotBrain Brain { get; }
        IHsbotLog Log { get; }
        Func<OutboundResponse, Task> SendMessage { get; }
        IChatMessageTextFormatter MessageTextFormatter { get; }
        ISystemClock SystemClock { get; }
        ITumblrApiClient TumblrApiClient { get; }
    }
}