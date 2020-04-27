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
        private readonly string _bragAndAwardChannel = "CE9K4LTFD"; //Test room: GCT9TL8KA Brag room: CE9K4LTFD
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

                var nominator = await context.GetChatUserById(message.UserId);
                var nominees = celebration.GetNomineeUserIds(match);

                var successes = new List<(string FullName, string Key)>();
                foreach (var nomineeUserId in nominees)
                {
                    var nominee = await context.GetChatUserById(nomineeUserId);
                    if (!nominee.IsEmployee)
                    {
                        await context.SendResponse(":blush: Sorry, only employees can be celebrated.");
                        continue;
                    }

                    if (nominee.Id == nominator.Id)
                    {
                        await context.SendResponse(":disapproval: celebrating yourself is not allowed!");
                        continue;
                    }

                    var result = await celebration.Submit(nominator, nominee, match);
                    if (string.IsNullOrWhiteSpace(result.ErrorMessage))
                    {
                        successes.Add((nominee.FullName, result.Key));
                        continue;
                    }

                    await context.SendResponse($":doh: {result.ErrorMessage}");
                }

                if (successes.Any())
                {
                    var successfulNominees = string.Join(", ", successes.Select(x => $"{x.FullName} [{x.Key}]"));
                    var successMessage = $"Your celebration for {successfulNominees} was successfully retrieved and processed!";

                    await context.SendResponse(successMessage);
                }

                if (message.Channel != _bragAndAwardChannel && successes.Any())
                {
                    var hvaSuccessMessage = new OutboundResponse
                    {
                        Channel = _bragAndAwardChannel,
                        Text = celebration.GetAwardRoomMessage(successes, nominator.FullName, match),
                        UserId = message.BotId,
                    };

                    await context.SendMessage(hvaSuccessMessage);
                }
            }
        }
    }
}