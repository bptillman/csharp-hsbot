using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Hsbot.Core.ApiClients
{
    public interface ITumblrApiClient
    {
        Task<TumblrPhoto[]> GetPhotos(string blogUrl);
    }

    public class TumblrPhoto
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public string Url { get; set; }
    }

    public class TumblrApiClient : ITumblrApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHsbotConfig _hsbotConfig;

        public TumblrApiClient(HttpClient httpClient, IHsbotConfig hsbotConfig)
        {
            _httpClient = httpClient;
            _hsbotConfig = hsbotConfig;
        }

        public async Task<TumblrPhoto[]> GetPhotos(string blogUrl)
        {
            var requestUrl = $"https://api.tumblr.com/v2/blog/{blogUrl}/posts/photo?limit=50&api_key={_hsbotConfig.TumblrApiKey}";
            var response = await _httpClient.GetStringAsync(requestUrl);

            var photoResponse = JObject.Parse(response);
            var posts = photoResponse["response"]["posts"];

            var photos = posts.Select( p => p.SelectToken("photos[0].original_size"));
            var result = photos.Select(p => p.ToObject<TumblrPhoto>()).ToArray();

            return result;
        }
    }
}
