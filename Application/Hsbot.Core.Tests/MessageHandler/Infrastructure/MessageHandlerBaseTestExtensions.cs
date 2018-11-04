using System.Linq;
using System.Threading.Tasks;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Messaging;

namespace Hsbot.Core.Tests.MessageHandler.Infrastructure
{
    public static class MessageHandlerBaseTestExtensions
    {
        public static Task<BotProvidedServicesFake> HandleAsync(this MessageHandlerBase handler, string messageText = "fake message text")
        {
            var message = handler.GetTestMessageThatWillBeHandled(messageText);
            return handler.HandleAsync(message);
        }

        public static async Task<BotProvidedServicesFake> HandleAsync(this IInboundMessageHandler handler, InboundMessage message)
        {
            var context = new BotMessageContextFake(message);
            await handler.HandleAsync(context);

            return handler.BotProvidedServices as BotProvidedServicesFake;
        }

        /// <returns>
        /// An instance of InboundMessage containing the provided message text, where the bot is mentioned
        /// and the message is in a channel on which the handler responds.
        /// </returns>
        public static InboundMessage GetTestMessageThatWillBeHandled(this MessageHandlerBase handler, string messageText)
        {
            var message = new InboundMessage
            {
                BotIsMentioned = true,
                BotId = "test",
                BotName = "test",
                Channel = handler.GetTestMessageChannel(),
                ChannelName = handler.GetTestMessageChannel(),
                FullText = messageText,
                MessageRecipientType = MessageRecipientType.Channel,
                RawText = messageText,
                TextWithoutBotName = messageText,
                UserChannel = "",
                UserEmail = "test@test.com",
                UserId = "nobody",
                Username = "nobody"
            };
            return message;
        }

        public static string GetTestMessageChannel(this MessageHandlerBase handler)
        {
            if (handler.TargetedChannels != null && handler.TargetedChannels.Any())
                return handler.TargetedChannels.First();

            return "Fake channel";
        }
    }
}
