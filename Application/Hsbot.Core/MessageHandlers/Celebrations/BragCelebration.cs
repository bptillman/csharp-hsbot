using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hsbot.Core.ApiClients;
using Hsbot.Core.Messaging;

namespace Hsbot.Core.MessageHandlers.Celebrations
{
    public class BragCelebration : ICelebration
    {
        private readonly Regex _bragRegex = new Regex(@"brag* (on|about)? *((<@([a-zA-Z0-9.-]+)>( *, * and *| *, *& *| *, *| *and *| *& *| *)?)+) (.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly IJiraApiClient _jiraApiClient;

        public BragCelebration(IJiraApiClient jiraApiClient)
        {
            _jiraApiClient = jiraApiClient;
        }

        public string EmployeesOnlyMessage => ":blush: Sorry, only employees can get brags.";
        public string SelfAggrandizingMessage => ":disapproval: bragging on yourself is not allowed!";

        public MessageHandlerDescriptor CommandDescriptor => new MessageHandlerDescriptor
        {
            Command = "hsbot brag on | about <<@coworker>> <bragText>",
            Description = "<bragText> and at least one <<@coworker>> are required\n\tmultiple <<@coworker>>'s can be bragged simultaneously when separated by spaces, and, &, or commas (Oxford or otherwise)"
        };

        public Match GetMatch(InboundMessage message) => message.Match(_bragRegex);

        public IEnumerable<string> GetNomineeUserIds(Match match) => ParseUsers(match.Groups[2].Value.Trim());

        public string GetRoomMessage(IEnumerable<(string FullName, string Key)> successes)
        {
            var successfulNominees = string.Join(", ", successes.Select(x => $"{x.FullName} [{x.Key}]"));
            return $"Your brag about {successfulNominees} was successfully retrieved and processed!";
        }

        public async Task<(string ErrorMessage, string Key)> Submit(IUser nominator, IUser nominee, Match match)
        {
            var reason = GetReason(match);
            var jiraResponse = await _jiraApiClient.SubmitBrag(nominator, nominee, reason);

            var errorMessage = jiraResponse.Failed
                ? $"Brag about {nominee.FullName} failed: {jiraResponse.FailureResponse}"
                : string.Empty;
            return (errorMessage, jiraResponse.Key);
        }

        public string GetAwardRoomMessage(IEnumerable<(string FullName, string Key)> successes, string nominatorName, Match match) =>
            string.Join('\n', successes.Select(x => $"Kudos to *{x.FullName}* {GetReason(match)} bragged by: _{nominatorName}_ [{x.Key}]"));

        private static string GetReason(Match match) => match.Groups[6].Value.Trim();

        private IEnumerable<string> ParseUsers(string peopleToBragOn)
        {
            var peopleToBragOnWithoutBrackets = peopleToBragOn.Replace("<", string.Empty).Replace(">", string.Empty);
            var regexForAllDelimiters = new Regex("( and |[, &])");
            var cleaned = regexForAllDelimiters.Replace(peopleToBragOnWithoutBrackets, string.Empty);
            return cleaned.Split("@").Where(x => !string.IsNullOrWhiteSpace(x)).Distinct();
        }
    }
}
