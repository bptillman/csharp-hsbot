using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hsbot.Core.MessageHandlers.Helpers;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public class DndMessageHandler : MessageHandlerBase
    {
        public DndMessageHandler(IRandomNumberGenerator randomNumberGenerator) : base(randomNumberGenerator)
        {
        }

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            return new[]
            {
                new MessageHandlerDescriptor { Command = "roll dX", Description = "roll an X-sided die (e.g. `d20`)" },
                new MessageHandlerDescriptor { Command = "roll YdX", Description = "roll Y X-sided dice (e.g. `2d6`)" },
                new MessageHandlerDescriptor { Command = "roll YdX+Z", Description = "roll Y X-sided dice, adding a modifier (e.g. `1d4+2`)" },
            };
        }

        public static Regex RollRegex = new Regex(@"roll (\d+)?d(\d+)(\+(\d+))?", RegexOptions.Compiled);

        protected override bool CanHandle(InboundMessage message)
        {
            return message.IsMatch(RollRegex);
        }

        public override Task HandleAsync(InboundMessage message)
        {
            var matches = message.Match(RollRegex);
            var quantity = matches.Groups[1].CaptureToInt() ?? 1;
            var die = matches.Groups[2].CaptureToInt().Value;
            var mod = matches.Groups[4].CaptureToInt() ?? 0;

            var total = mod;
            var rolls = new int[quantity];

            for (var i = 0; i < quantity; ++i)
            {
                rolls[i] = RandomNumberGenerator.Generate(1, die + 1);
                total += rolls[i];
            }

            return SendMessage(message.CreateResponse($"rolled a {total} ({string.Join(", ", rolls)})+{mod}"));
        }
    }
}
