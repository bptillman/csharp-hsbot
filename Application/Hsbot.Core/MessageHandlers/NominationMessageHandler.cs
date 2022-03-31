using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hsbot.Core.MessageHandlers.Celebrations;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public class NominationMessageHandler : MessageHandlerBase
    {
        private readonly string _bragAndAwardChannel = "#brags-and-awards";
        private readonly IEnumerable<ICelebration> _celebrations;

        public NominationMessageHandler(IEnumerable<ICelebration> celebrations, IRandomNumberGenerator randomNumberGenerator) : base(randomNumberGenerator)
        {
            _celebrations = celebrations;
        }

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            return _celebrations.Select(x => x.CommandDescriptor);
        }

        protected override bool CanHandle(InboundMessage message)
        {
            return _celebrations.Any(x => x.GetMatch(message).Success);
        }

        public override async Task HandleAsync(IInboundMessageContext context)
        {
            var message = context.Message;

            foreach (var celebration in _celebrations)
            {
                var match = celebration.GetMatch(message);
                if (!match.Success)
                {
                    continue;
                }

                var celebrationType = celebration.GetType() == typeof(BragCelebration) ? "brag" : "HVA";
                var responseMessage = new OutboundResponse
                {
                    Channel = _bragAndAwardChannel,
                    Text =
                        $"That's a great {celebrationType} {message.Username}, but right now I am unable to process brags and HVA's via Slack. " +
                        " So to make sure this gets captured so we can celebrate it in our weekly meetings, could you please submit it here: https://headspring.atlassian.net/servicedesk/customer/portal/7 . :thanks:",
                    UserId = message.BotId
                };
                await context.SendResponse(responseMessage);
            }
        }
    }
}