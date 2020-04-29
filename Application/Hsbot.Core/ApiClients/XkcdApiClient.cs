using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Hsbot.Core.ApiClients
{
    public interface IXkcdApiClient
    {
        Task<XkcdInfo> GetInfo(string id = null);
    }

    public class XkcdApiClient: IXkcdApiClient
    {
        private const string BaseUrl = "https://xkcd.com/";
        private const string JsonTag = "info.0.json";

        private readonly HttpClient _httpClient;

        public XkcdApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<XkcdInfo> GetInfo(string id = null)
        {
            var url = string.IsNullOrEmpty(id) ? $"{BaseUrl}{JsonTag}" : $"{BaseUrl}{id}/{JsonTag}";
            string response;

            try
            {
                response = await _httpClient.GetStringAsync(url);
            }
            catch
            {
                throw new Exception("Error: Service is not working.");
            }

            var xkcdResponse = JObject.Parse(response);
            return xkcdResponse.ToObject<XkcdInfo>();
        }
    }

    public class XkcdInfo
    {
        public string Num { get; set; }
        public string Title { get; set; }
        public string Img { get; set; }
        public string Alt { get; set; }
    }
}
