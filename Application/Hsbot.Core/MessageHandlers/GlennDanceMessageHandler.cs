using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public class GlennDanceMessageHandler : MessageHandlerBase
    {
        private const string MakeGlennDanceCommand = "make glenn dance";
        private const string GlennDanceBombCommand = "glenn dance bomb";

        public override string[] CannedResponses => new[]
        {
            "https://i.imgur.com/0G6Bdvs.gif",
            "https://i.imgur.com/3YveU2X.gif",
            "https://i.imgur.com/88vefL9.gif",
            "https://i.imgur.com/HAuH8ly.gif",
            "https://i.imgur.com/cTxSiI1.gif",
            "https://i.imgur.com/flyMHi1.gif"
        };

        public GlennDanceMessageHandler(IRandomNumberGenerator randomNumberGenerator) : base(randomNumberGenerator)
        {
        }

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            return new[]
            {
                new MessageHandlerDescriptor { Command = "make glenn dance", Description = "Spread the joy of Glenn dancing to everyone."},
                new MessageHandlerDescriptor { Command = "glenn dance bomb <N>", Description = "Spread the joy of Glenn dancing to everyone - N times."},
            };
        }

        protected override bool CanHandle(InboundMessage message)
        {
            return message.StartsWith(MakeGlennDanceCommand) || message.StartsWith(GlennDanceBombCommand);
        }

        public override async Task HandleAsync(InboundMessage message)
        {
            var repliesNumber = 0;

            if (message.StartsWith(MakeGlennDanceCommand))
            {
                repliesNumber = 1;
            }

            else if (message.StartsWith(GlennDanceBombCommand))
            {
                repliesNumber = CannedResponses.Length;

                var bombCountText = message.TextWithoutBotName.Substring(GlennDanceBombCommand.Length, message.TextWithoutBotName.Length - GlennDanceBombCommand.Length);
                if (int.TryParse(bombCountText, out var bombCount))
                {
                    if (bombCount >= 0) repliesNumber = bombCount;
                }
            }

            for (var i = 0; i < repliesNumber; i++)
            {
                await SendMessage(message.CreateResponse(GetRandomCannedResponse()));
            }
        }
    }
}
