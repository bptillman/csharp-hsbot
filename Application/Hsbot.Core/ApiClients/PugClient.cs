using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Hsbot.Core.ApiClients
{
    public interface IPugClient
    {
        Task<PugInfo[]> GetPugs(int count = 1);
    }

    public class PugClient : IPugClient
    {
        private readonly HttpClient _httpClient;

        public PugClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PugInfo[]> GetPugs(int count = 1)
        {
            try
            {
                var requestUrl = "http://pugme.herokuapp.com/bomb?count=" + count;
                var response = await _httpClient.GetStringAsync(requestUrl);
                var pugResponse = JObject.Parse(response);
                var pugs = pugResponse["pugs"];
                return pugs.Select(i => new PugInfo{ Img = i.ToString()}).ToArray();
            }
            catch (Exception e)
            {
                throw new Exception($"Error: Service is not working. {e}");
            }
        }
    }

    public class PugInfo
    {
        public string Img { get; set; }
    }
}
