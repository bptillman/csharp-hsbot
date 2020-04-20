using System;
using System.Threading.Tasks;
using Hsbot.Core.Messaging;

namespace Hsbot.Core.Connection
{
    public interface IHsbotChatConnector
    {
        Task Connect();
        Task Disconnect();
        Task SendMessage(OutboundResponse response);
        Task<IChatUser> GetChatUserById(string userId);

        IObservable<IHsbotChatConnector> Disconnected { get; }
        IObservable<IHsbotChatConnector> Reconnecting { get; }
        IObservable<IHsbotChatConnector> Reconnected { get; }
        IObservable<Task<InboundMessage>> MessageReceived { get; }
    }
}
