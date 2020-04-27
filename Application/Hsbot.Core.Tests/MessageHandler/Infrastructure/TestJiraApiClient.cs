using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Core.ApiClients;

namespace Hsbot.Core.Tests.MessageHandler.Infrastructure
{
    public class TestJiraApiClient : IJiraApiClient
    {
        public SubmissionResponse SubmissionResponse { get; set; }
        public string ErrorMessage { get; set; }
        public IEnumerable<IUser> Users { get; set; }

        Task<SubmissionResponse> IJiraApiClient.SubmitHva(IUser nominator, IUser nominee, string description, string awardType)
        {
            return Task.FromResult(SubmissionResponse);
        }

        public Task<SubmissionResponse> SubmitBrag(IUser nominator, IUser nominee, string description)
        {
            return Task.FromResult(SubmissionResponse);
        }
    }
}
