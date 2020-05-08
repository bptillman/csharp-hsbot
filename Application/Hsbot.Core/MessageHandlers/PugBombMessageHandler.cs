using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hsbot.Core.ApiClients;
using Hsbot.Core.MessageHandlers.Helpers;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public class PugBombMessageHandler : MessageHandlerBase
    {
        private readonly IPugClient _pugClient;

        public static Regex PugBombRegex = new Regex(@"pug bomb (\d+)", RegexOptions.Compiled);

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
            return message.StartsWith("pug me") || message.StartsWith("pug bomb");
        }

        public override async Task HandleAsync(IInboundMessageContext context)
        {
            var numberOfPugs = 5;
            var message = context.Message;
            if (message.StartsWith("pug me"))
            {
                numberOfPugs = 1;
            }
            else
            {
                var match = message.Match(PugBombRegex);
                if (match.Success)
                {
                    numberOfPugs = match.Groups[1].CaptureToInt() ?? 5;
                }
            }

            var pugs = await _pugClient.GetPugs(numberOfPugs);

            foreach (var pug in pugs)
            {
                await context.SendResponse(pug.Img);
            }
        }
    }
}
