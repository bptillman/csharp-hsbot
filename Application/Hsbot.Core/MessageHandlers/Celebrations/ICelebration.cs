using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hsbot.Core.Messaging;

namespace Hsbot.Core.MessageHandlers.Celebrations
{
    public interface ICelebration
    {
        MessageHandlerDescriptor CommandDescriptor { get; }
        Match GetMatch(InboundMessage message);
        IEnumerable<string> GetNomineeUserIds(Match match);
        Task<(string ErrorMessage, string Key)> Submit(IUser nominatorJiraUser, IUser nomineeJiraUser, Match match);
        string GetAwardRoomMessage(IEnumerable<(string FullName, string Key)> successes, string nominatorName, Match match);
    }
}
