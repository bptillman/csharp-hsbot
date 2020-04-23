using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hsbot.Core.ApiClients;

namespace Hsbot.Core.Tests.MessageHandler.Infrastructure
{
    public class TestJiraUser : JiraUser
    {
        public string Email { get; set; }
    }

    public class TestJiraApiClient : IJiraApiClient
    {
        public string HvaSuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
        public IEnumerable<TestJiraUser> Users { get; set; }

        Task<HvaResponse> IJiraApiClient.SubmitHva(JiraUser nominator, JiraUser nominee, string description, string awardType)
        {
            return Task.FromResult(new HvaResponse
            {
                Message = HvaSuccessMessage,
                Failed = false,
                HvaKey = "theKey",
            });
        }

        public Task<JiraUser> GetUser(string search)
        {
            var user = Users.FirstOrDefault(
                           x => x.DisplayName.Equals(search, StringComparison.InvariantCultureIgnoreCase)
                                || x.Email.Equals(search, StringComparison.InvariantCultureIgnoreCase))
                       ?? new JiraUser {Error = ErrorMessage};

            return Task.FromResult(user);
        }
    }
}
