using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Hsbot.Core.ApiClients
{
    public interface IDictionaryApiClient
    {
        Task<DictionaryResponse> GetDefinition(string searchTerm);
    }

    public class DictionaryResponse
    {
        public string[] Recommendations { get; set; }
        public string[] Definitions { get; set; }

        public bool HasDefinition => Definitions?.Any() ?? false;
        public bool HasRecommendations => Recommendations?.Any() ?? false;
    }

    public class DictionaryApiClient : IDictionaryApiClient
    {
        private readonly string _apiUrl = "https://www.dictionaryapi.com/api/v3/references/collegiate/json/{0}?key={1}";
        private readonly IHsbotConfig _hsbotConfig;
        private readonly HttpClient _httpClient;

        public DictionaryApiClient(IHsbotConfig hsbotConfig, HttpClient httpClient)
        {
            _hsbotConfig = hsbotConfig;
            _httpClient = httpClient;
        }

        public async Task<DictionaryResponse> GetDefinition(string searchTerm)
        {
            var result = new DictionaryResponse();
            var response = await _httpClient.GetStringAsync(GetUrl(searchTerm));
            var baseArray = JArray.Parse(response);

            var id = baseArray[0].SelectToken("meta.id", false);

            if (id == null)
            {
                result.Recommendations = baseArray.Select(x => x.ToString()).ToArray();
                return result;
            }

            result.Definitions = baseArray.Where(x =>
            {
                var fullId = x.SelectToken("meta.id", false).ToString();
                var word = fullId.Split(':')[0];

                return word == searchTerm;
            }).SelectMany(x =>
            {
                var label = x.SelectToken("fl").ToString();
                return x["shortdef"].Select(def => $"({label}) {def}");
            }).ToArray();

            return result;
        }

        private string GetUrl(string searchTerm) => string.Format(_apiUrl, searchTerm, _hsbotConfig.DictionaryApiKey);
    }
}
