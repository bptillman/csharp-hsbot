using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Hsbot.Core.ApiClients
{
    public interface ISimpsonApiClient
    {
        Task<FrinkiacImage[]> GetImages(string url);
    }

    public class FrinkiacImage
    {
        public long Id { get; set; }
        public string Episode { get; set; }
        public long TimeStamp { get; set; }
    }

    public class SimpsonApiClient : ISimpsonApiClient
    {
        private readonly HttpClient _httpClient;

        public SimpsonApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<FrinkiacImage[]> GetImages(string url)
        {
            try
            {
                var response = await _httpClient.GetStringAsync(url);
                var arrayOfImages = JArray.Parse(response);
                return arrayOfImages.Select(i => i.ToObject<FrinkiacImage>()).ToArray();
            }
            catch
            {
                throw new Exception($"Error: Service is not working.");
            }
        }
    }
}
