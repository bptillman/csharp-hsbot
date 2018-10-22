using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Slack.Core.Messaging;
using Hsbot.Slack.Core.Random;

namespace Hsbot.Slack.Core.MessageHandlers
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

        public override Task HandleAsync(BotMessageContext context)
        {
            return context.ReplyToChannel("Pong!");
        }
    }
}
