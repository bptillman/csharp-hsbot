using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hsbot.Core.ApiClients;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public class NominationMessageHandler : MessageHandlerBase
    {
        private readonly string _bragAndAwardChannel = "CE9K4LTFD"; //Test room: GCT9TL8KA Brag room: CE9K4LTFD
        private readonly Regex _nominationRegex = new Regex(@"hva *(to *|for *)?<@([a-zA-Z0-9.-]+)> *for *(DFE|PAV|COM|PLG|OWN|GRIT|HUMILITY|CANDOR|CURIOSITY|AGENCY) (.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly IJiraApiClient _jiraApiClient;

        private readonly string[] _errorBarks = {
            "My time circuits must be shorting out, I couldn't do that :sad_panda:, please don't let me get struck by lightning :build:",
            ":shrug: What you requested should have worked, BUT it didn't",
            "Bad news: it didn't work :kaboom:; good news: I'm alive! I'm alive! :awesome: Wait, no...that is Johnny # 5, there is no good news :evil_burns:",
            "https://media.giphy.com/media/owRSsSHHoVYFa/giphy.gif"
        };

        public NominationMessageHandler(IJiraApiClient jiraApiClient, IRandomNumberGenerator randomNumberGenerator) : base(randomNumberGenerator)
        {
            _jiraApiClient = jiraApiClient;
        }

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            yield return new MessageHandlerDescriptor
            {
                Command = "hsbot hva [to|for] @coworker for awardAcronym nominationText",
                Description = "Coworker and nominationText are required, awardAcronym must be one of:\n\tDFE or GRIT (Drive for Excellence)\n\tPAV or HUMILITY (People are Valued)\n\tCOM or CANDOR (Honest Communication)\n\tPLG or CURIOSITY (Passion for Learning and Growth)\n\tOWN or AGENCY (Own Your Experience)"
            };
        }

        protected override bool CanHandle(InboundMessage message)
        {
            return message.IsMatch(_nominationRegex);
        }

        public override async Task HandleAsync(IInboundMessageContext context)
        {
            var message = context.Message;
            var matchResult = message.Match(_nominationRegex);

            var nomineeUserId = matchResult.Groups[2].Value.Trim();
            var awardType = matchResult.Groups[3].Value.Trim();
            var reason = matchResult.Groups[4].Value.Trim();

            var nominee = await context.GetChatUserById(nomineeUserId);
            var nominator = await context.GetChatUserById(message.UserId);

            if (!nominee.IsEmployee)
            {
                await context.SendMessage(message.CreateResponse(":blush: Sorry, only employees can be nominated."));
                return;
            }

            if(nominee.Id == nominator.Id)
            {
                await context.SendMessage(message.CreateResponse(":disapproval: nominating yourself is not allowed!"));
                return;
            }

            var nomineeJiraUser = await _jiraApiClient.GetUser(nominee.Email);
            if (nomineeJiraUser.HasError)
            {
                nomineeJiraUser = await _jiraApiClient.GetUser(nominee.FullName);
            }

            if (nomineeJiraUser.HasError)
            {
                await context.SendMessage(message.CreateResponse($":doh: {nomineeJiraUser.Error}"));
                return;
            }

            var nominatorJiraUser = await _jiraApiClient.GetUser(nominator.Email);
            if (nominatorJiraUser.HasError)
            {
                nominatorJiraUser = await _jiraApiClient.GetUser(nominator.FullName);
            }

            if (nominatorJiraUser.HasError)
            {
                await context.SendMessage(message.CreateResponse($":doh: {nominatorJiraUser.Error}"));
                return;
            }

            var response = await _jiraApiClient.SubmitHva(nominatorJiraUser, nomineeJiraUser, reason, awardType);

            if (response.Failed)
            {
                await context.SendMessage(message.CreateResponse(RandomNumberGenerator.GetRandomValue(_errorBarks)));
                return;
            }

            await context.SendMessage(message.CreateResponse(response.Message));

            if (message.Channel != _bragAndAwardChannel)
            {
                var hvaSuccessMessage = new OutboundResponse
                {
                    Channel = _bragAndAwardChannel,
                    Text = $"{nominee.FullName} exhibits *_{awardType}_*\n{reason}\nnominated by: _{nominator.FullName}_\n{response.HvaKey}",
                    UserId = message.BotId,
                };

                await context.SendMessage(hvaSuccessMessage);
            }
        }
    }
}
