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
        Task<HvaResponse> SubmitHva(
            JiraUser nominator,
            JiraUser nominee,
            string description,
            string awardType);

        Task<JiraUser> GetUser(string search);
    }

    public class JiraUser
    {
        public string AccountId { get; set; }
        public string DisplayName { get; set; }
        public string Error { get; set; }
        public bool HasError => !string.IsNullOrEmpty(Error);
    }

    public class HvaResponse
    {
        public string Message { get; set; }
        public string HvaKey { get; set; }
        public bool Failed { get; set; }
    }

    public class JiraApiClient : IJiraApiClient
    {
        private readonly string _projectKey = "NOM";
        private readonly string _projectId = "14701";
        private readonly string _hvaId = "11303";
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

        public async Task<HvaResponse> SubmitHva(JiraUser nominator, JiraUser nominee, string description, string awardType)
        {
            var awardText = GetHvaJiraAwardText(awardType);
            var body = new
            {
                fields = new
                {
                    project = new {key = _projectKey, id = _projectId},
                    issuetype = new {id = _hvaId},
                    customfield_12100 = new {id = nominee.AccountId},
                    customfield_12101 = new {value = awardText},
                    description = description,
                    summary = $"{nominator.DisplayName} nominates {nominee.DisplayName} on {_systemClock.UtcNow:MMM dd, yyyy}",
                    reporter = new {id = nominator.AccountId}
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/issue");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", _hsbotConfig.JiraApiKey);
            request.Content = new CapturedJsonContent(JsonConvert.SerializeObject(body));
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return new HvaResponse {Failed = true};
            }

            dynamic content = JObject.Parse(await response.Content.ReadAsStringAsync());
            var issueKey = content.key;

            return new HvaResponse
            {
                Message = $"Your nomination of {nominee.DisplayName} for {awardType} was successfully retrieved and processed! [{issueKey}]"
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

        public async Task<JiraUser> GetUser(string search)
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
    }
}
