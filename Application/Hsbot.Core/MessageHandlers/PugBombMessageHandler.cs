using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Core.ApiClients;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public class PugBombMessageHandler : MessageHandlerBase
    {
        private readonly IPugClient _pugClient;

        public PugBombMessageHandler(IRandomNumberGenerator randomNumberGenerator, IPugClient pugClient) : base(randomNumberGenerator)
        {
            _pugClient = pugClient;
        }

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            return new List<MessageHandlerDescriptor>()
            {
                new MessageHandlerDescriptor { Command = "pug me", Description = "Receive a pug"},
                new MessageHandlerDescriptor { Command = "pug bomb <N>", Description = "Get N pugs"}
            };
        }

        protected override bool CanHandle(InboundMessage message)
        {
            throw new NotImplementedException();
        }

        public override Task HandleAsync(IInboundMessageContext context)
        {
            throw new NotImplementedException();
        }
    }
}
