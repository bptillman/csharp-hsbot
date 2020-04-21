using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public class PingMessageHandler : MessageHandlerBase
    {
        private const string CommandText = "ping";

        public PingMessageHandler(IRandomNumberGenerator randomNumberGenerator) : base(randomNumberGenerator)
        {
        }

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            return new[]
            {
                new MessageHandlerDescriptor
                {
                    Command = CommandText,
                    Description = "Replies to user who sent the message with 'Pong!'"
                }
            };
        }

        protected override bool CanHandle(InboundMessage message)
        {
            return message.StartsWith(CommandText);
        }

        public override Task HandleAsync(IInboundMessageContext context)
        {
            var message = context.Message;
            var response = message.CreateResponse("Pong!");

            return context.SendMessage(response);
        }
    }
}
