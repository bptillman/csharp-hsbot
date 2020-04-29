using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Core.Messaging;

namespace Hsbot.Core.Tests
{
    public class FakeMessageHandler : IInboundMessageHandler
    {
        public bool HandlesMessage { get; set; } = true;
        public List<IInboundMessageContext> HandledMessages { get; } = new List<IInboundMessageContext>();
        public Action<IInboundMessageContext> HandlerAction { get; set; } = c => { };

        public IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            yield break;
        }

        public HandlesResult Handles(InboundMessage message)
        {
            return new HandlesResult
            {
                HandlesMessage = HandlesMessage
            };
        }

        public Task HandleAsync(IInboundMessageContext messageContext)
        {
            HandledMessages.Add(messageContext);
            HandlerAction(messageContext);
            return Task.CompletedTask;
        }
    }
}