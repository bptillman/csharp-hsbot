using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public class Magic8MessageHandler : MessageHandlerBase
    {
        public Magic8MessageHandler(IRandomNumberGenerator randomNumberGenerator) : base(randomNumberGenerator)
        {
        }

        public override string[] TargetedChannels => AllChannels;
        public override bool DirectMentionOnly => true;

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            yield return new MessageHandlerDescriptor { Command = "will <ask your question with a question mark>?", Description = "Ask the Magic 8 ball to predict the future!!" };
        }

        public override string[] CannedResponses => new []
        {
            "It is certain",
            "It is decidedly so",
            "Without a doubt",
            "Yes definitely",
            "You may rely on it",
            "As I see it, yes",
            "Most likely",
            "Outlook good",
            "Yes",
            "Signs point to yes",
            "Reply hazy try again",
            "Ask again later",
            "Better not tell you now",
            "Cannot predict now",
            "Concentrate and ask again",
            "Do not count on it",
            "My reply is no",
            "My sources say no",
            "Outlook not so good",
            "Very doubtful"
        };

        public override Task HandleAsync(IInboundMessageContext context)
        {
            return context.SendResponse($":magic8ball: {GetRandomCannedResponse()}");
        }

        protected override bool CanHandle(InboundMessage message)
        {
            return message.StartsWith("will") && message.EndsWith("?");
        }
    }
}