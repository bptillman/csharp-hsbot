using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Flurl.Http.Content;
using Hsbot.Core.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hsbot.Core.ApiClients
{
    public interface IJiraApiClient
    {
        Task<SubmissionResponse> SubmitHva(
            IUser nominator,
            IUser nominee,
            string description,
            string awardType);

        Task<SubmissionResponse> SubmitBrag(
            IUser nominator,
            IUser nominee,
            string description);
    }

    public class SubmissionResponse
    {
        public string Key { get; set; }
        public bool Failed { get; set; }
        public string FailureResponse { get; set; }
    }

    public class JiraApiClient : IJiraApiClient
    {
        private readonly string _projectKey = "NOM";
        private readonly string _projectId = "14701";
        private readonly string _hvaId = "11303";
        private readonly string _bragId = "11304";
        private readonly string _baseUrl = "https://headspring.atlassian.net/rest/api/2/";

        private readonly HttpClient _httpClient;
        private readonly IHsbotConfig _hsbotConfig;
        private readonly ISystemClock _systemClock;

        public JiraApiClient(HttpClient httpClient, IHsbotConfig hsbotConfig, ISystemClock systemClock)
        {
            _httpClient = httpClient;
            _hsbotConfig = hsbotConfig;
            _systemClock = systemClock;
        }

        public async Task<SubmissionResponse> SubmitHva(IUser nominator, IUser nominee, string description, string awardType)
        {
            var nominatorJiraUser = await GetJiraUser(nominator);
            if (nominatorJiraUser.HasError)
            {
                return new SubmissionResponse { Failed = true, FailureResponse = nominatorJiraUser.Error };
            }

            var nomineeJiraUser = await GetJiraUser(nominee);
            if (nomineeJiraUser.HasError)
            {
                return new SubmissionResponse { Failed = true, FailureResponse = nomineeJiraUser.Error };
            }

            var body = new
            {
                fields = new
                {
                    project = new {key = _projectKey, id = _projectId},
                    issuetype = new {id = _hvaId},
                    customfield_12100 = new {id = nomineeJiraUser.AccountId},
                    customfield_12101 = new {value = GetHvaJiraAwardText(awardType)},
                    description = description,
                    summary = $"{nominatorJiraUser.DisplayName} nominates {nomineeJiraUser.DisplayName} on {_systemClock.UtcNow:MMM dd, yyyy}",
                    reporter = new {id = nominatorJiraUser.AccountId}
                }
            };

            return await SendSubmissionToJira(body);
        }

        public async Task<SubmissionResponse> SubmitBrag(IUser nominator, IUser nominee, string description)
        {
            var nominatorJiraUser = await GetJiraUser(nominator);
            if (nominatorJiraUser.HasError)
            {
                return new SubmissionResponse {Failed = true, FailureResponse = nominatorJiraUser.Error};
            }

            var nomineeJiraUser = await GetJiraUser(nominee);
            if (nomineeJiraUser.HasError)
            {
                return new SubmissionResponse {Failed = true, FailureResponse = nomineeJiraUser.Error};
            }

            var body = new
            {
                fields = new
                {
                    project = new {key = _projectKey, id = _projectId},
                    issuetype = new {id = _bragId},
                    customfield_12100 = new {id = nomineeJiraUser.AccountId},
                    description = description,
                    summary = $"{nominatorJiraUser.DisplayName} brags about {nomineeJiraUser.DisplayName} on {_systemClock.UtcNow:MMM dd, yyyy}",
                    reporter = new {id = nominatorJiraUser.AccountId}
                }
            };

            return await SendSubmissionToJira(body);
        }

        private async Task<SubmissionResponse> SendSubmissionToJira(object body)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/issue");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", _hsbotConfig.JiraApiKey);
            request.Content = new CapturedJsonContent(JsonConvert.SerializeObject(body));
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return new SubmissionResponse { Failed = true };
            }

            dynamic content = JObject.Parse(await response.Content.ReadAsStringAsync());
            var issueKey = content.key;

            return new SubmissionResponse
            {
                Key = issueKey,
            };
        }

        private string GetHvaJiraAwardText(string awardType)
        {
            switch (awardType.ToLower())
            {
                case "dfe":
                case "grit":
                    return "Drive for Excellence";
                case "pav":
                case "humility":
                    return "People are Valued";
                case "com":
                case "candor":
                    return "Honest Communication";
                case "plg":
                case "curiosity":
                    return "Passion for Learning and Growth";
                case "own":
                case "agency":
                    return "Own Your Experience";
                default:
                    return null;
            }
        }

        private async Task<JiraUser> GetJiraUser(IUser user)
        {
            var nomineeJiraUser = await GetUser(user.Email);
            if (nomineeJiraUser.HasError)
            {
                nomineeJiraUser = await GetUser(user.FullName);
            }

            return nomineeJiraUser;
        }

        private async Task<JiraUser> GetUser(string search)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/user/picker?query={search}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", _hsbotConfig.JiraApiKey);// Convert.ToBase64String(Encoding.UTF8.GetBytes(_hsbotConfig.JiraApiToken)));

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = JObject.Parse(await response.Content.ReadAsStringAsync());

                if (!content["users"].Any())
                {
                    return new JiraUser
                    {
                        Error = $"JIRA does not have the proper user information for {search}! Please make sure the user is correctly configured in JIRA to continue"
                    };
                }

                if (content["users"].Count() > 1)
                {
                    return new JiraUser
                    {
                        Error = $"We found more than one {search} in JIRA?! Please be more specific to proceed"
                    };
                }

                var user = content["users"].First;

                return new JiraUser
                {
                    AccountId = (string)user["accountId"],
                    DisplayName = (string)user["displayName"],
                };
            }

            return new JiraUser
            {
                Error = "Something went wrong...Jira user search failed."
            };
        }

        private class JiraUser
        {
            public string AccountId { get; set; }
            public string DisplayName { get; set; }
            public string Error { get; set; }
            public bool HasError => !string.IsNullOrEmpty(Error);
        }
    }
}
