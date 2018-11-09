using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

        private int? CaptureToInt(Capture capture)
        {
            return capture == null || string.IsNullOrEmpty(capture.Value)
                ? (int?)null
                : int.Parse(capture.Value);
        }

        public override Task HandleAsync(InboundMessage message)
        {
            var matches = message.Match(RollRegex);
            var quantity = CaptureToInt(matches.Groups[1]) ?? 1;
            var die = CaptureToInt(matches.Groups[2]).Value;
            var mod = CaptureToInt(matches.Groups[4]) ?? 0;

            var total = mod;
            var rolls = new int[quantity];

            for (var i = 0; i < quantity; ++i)
            {
                rolls[i] = RandomNumberGenerator.Generate(1, die + 1);
                total += rolls[i];
            }

            return ReplyToChannel(message, $"rolled a {total} ({string.Join(", ", rolls)})+{mod}");
        }
    }
}
