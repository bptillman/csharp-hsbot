using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hsbot.Core.ApiClients;
using Hsbot.Core.MessageHandlers.Helpers;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers.Celebrations
{
    public class NominationCelebration : ICelebration
    {
        private readonly Regex _nominationRegex = new Regex(@"hva *(to *|for *)?<@([a-zA-Z0-9.-]+)> *for *(DFE|PAV|COM|PLG|OWN|GRIT|HUMILITY|CANDOR|CURIOSITY|AGENCY) (.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly string[] _errorBarks =
        {
            "My time circuits must be shorting out, I couldn't do that :sad_panda:, please don't let me get struck by lightning :build:",
            ":shrug: What you requested should have worked, BUT it didn't",
            "Bad news: it didn't work :kaboom:; good news: I'm alive! I'm alive! :awesome: Wait, no...that is Johnny # 5, there is no good news :evil_burns:",
            "https://media.giphy.com/media/owRSsSHHoVYFa/giphy.gif"
        };

        private readonly IJiraApiClient _jiraApiClient;
        private readonly IRandomNumberGenerator _randomNumberGenerator;

        public NominationCelebration(IJiraApiClient jiraApiClient, IRandomNumberGenerator randomNumberGenerator)
        {
            _jiraApiClient = jiraApiClient;
            _randomNumberGenerator = randomNumberGenerator;
        }

        public string EmployeesOnlyMessage => ":blush: Sorry, only employees can be nominated.";
        public string SelfAggrandizingMessage => ":disapproval: nominating yourself is not allowed!";

        public MessageHandlerDescriptor CommandDescriptor => new MessageHandlerDescriptor
        {
            Command = "hsbot hva to | for <<@coworker>> for >awardAcronym> <nominationText>",
            Description = "<<@coworker>>, <awardAcronym> and <nominationText> are required, <awardAcronym> must be one of:\n\tDFE or GRIT (Drive for Excellence)\n\tPAV or HUMILITY (People are Valued)\n\tCOM or CANDOR (Honest Communication)\n\tPLG or CURIOSITY (Passion for Learning and Growth)\n\tOWN or AGENCY (Own Your Experience)"
        };

        public Match GetMatch(InboundMessage message) => message.Match(_nominationRegex);

        public IEnumerable<string> GetNomineeUserIds(Match match) => new[] {match.Groups[2].Value.Trim()};

        public string GetRoomMessage(IEnumerable<(string FullName, string Key)> successes)
        {
            var successfulNominees = string.Join(", ", successes.Select(x => $"{x.FullName} [{x.Key}]"));
            return $"Your nomination for {successfulNominees} was successfully retrieved and processed!";
        }

        public async Task<(string ErrorMessage, string Key)> Submit(IUser nominator, IUser nominee, Match match)
        {
            var (awardType, reason) = GetAwardTypeAndReason(match);
            var jiraResponse = await _jiraApiClient.SubmitHva(nominator, nominee, reason, awardType);

            var errorMessage = jiraResponse.Failed
                ? _randomNumberGenerator.GetRandomValue(_errorBarks)
                : string.Empty;
            return (errorMessage, jiraResponse.Key);
        }

        public string GetAwardRoomMessage(IEnumerable<(string FullName, string Key)> successes, string nominatorName, Match match)
        {
            var (awardType, reason) = GetAwardTypeAndReason(match);
            var (fullName, key) = successes.First();
            return $"{fullName} exhibits *_{awardType.ToJiraAwardText()}_* {reason} nominated by: _{nominatorName}_ [{key}]";
        }

        private static (string awardType, string reason) GetAwardTypeAndReason(Match match)
        {
            var awardType = match.Groups[3].Value.Trim();
            var reason = match.Groups[4].Value.Trim();
            return (awardType, reason);
        }
    }
}
