using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Hsbot.Core.ApiClients
{
    public interface IThesaurusApiClient
    {
        Task<ThesaurusResponse> LookUp(string searchTerm);
    }

    public class ThesaurusResponse
    {
        public string[] Definitions { get; set; }
        public string[] Synonyms { get; set; }
        public string[] Antonyms { get; set; }

        public bool HasDefinitions => Definitions?.Any() ?? false;
        public bool HasSynonyms => Synonyms?.Any() ?? false;
        public bool HasAntonyms => Antonyms?.Any() ?? false;

        public bool HasSomething => HasAntonyms || HasSynonyms || HasDefinitions;
    }

    public class ThesaurusApiClient :IThesaurusApiClient
    {
        private readonly string _apiUrl = "https://dictionaryapi.com/api/v3/references/thesaurus/json/{0}?key={1}";
        private readonly IHsbotConfig _hsbotConfig;
        private readonly HttpClient _httpClient;

        public ThesaurusApiClient(IHsbotConfig hsbotConfig, HttpClient httpClient)
        {
            _hsbotConfig = hsbotConfig;
            _httpClient = httpClient;
        }

        public async Task<ThesaurusResponse> LookUp(string searchTerm)
        {
            var result = new ThesaurusResponse();
            var response = await _httpClient.GetStringAsync(GetUrl(searchTerm));
            var baseArray = JArray.Parse(response);

            var id = baseArray[0].SelectToken("meta.id", false);

            if (id == null)
            {
                return result;
            }

            var matchingResults = baseArray.Where(x =>
            {
                var fullId = x.SelectToken("meta.id", false).ToString();
                var word = fullId.Split(':')[0];

                return word == searchTerm;
            }).ToList();

            result.Definitions = matchingResults.SelectMany(x =>
            {
                var label = x.SelectToken("fl").ToString();
                return x["shortdef"].Select(def => $"({label}) {def}");
            }).ToArray();

            result.Synonyms = matchingResults
                .SelectMany(x => x.SelectToken("meta.syns").SelectMany(synList => synList.Select(s => s.ToString()).ToArray()))
                .ToArray();

            result.Antonyms = matchingResults
                .SelectMany(x => x.SelectToken("meta.ants").SelectMany(antList => antList.Select(a => a.ToString()).ToArray()))
                .ToArray();

            return result;
        }

        private string GetUrl(string searchTerm) => string.Format(_apiUrl, searchTerm, _hsbotConfig.ThesaurusApiKey);
    }
}
