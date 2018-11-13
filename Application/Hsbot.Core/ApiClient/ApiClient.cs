namespace Hsbot.Core.ApiClient
{
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public interface IApiClient
    {
        Task<T> RequestDeserializedJson<T>(HttpMethod method, string url, HttpContent content = null);
    }

    public class ApiClient : IApiClient
    {
        public async Task<T> RequestDeserializedJson<T>(HttpMethod method, string url, HttpContent content = null)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(method, url))
            {
                if (content != null)
                    request.Content = content;

                using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                {
                    var stream = await response.Content.ReadAsStreamAsync();

                    if (response.IsSuccessStatusCode)
                        return DeserializeJsonFromStream<T>(stream);

                    var message = await StreamToStringAsync(stream);
                    throw new ApiException
                    {
                        StatusCode = (int)response.StatusCode,
                        Content = message
                    };
                }
            }
        }

        private static async Task<string> StreamToStringAsync(Stream stream)
        {
            if (stream == null) return null;

            string content;
            using (var streamReader = new StreamReader(stream))
                content = await streamReader.ReadToEndAsync();

            return content;
        }

        private static T DeserializeJsonFromStream<T>(Stream stream)
        {
            if (stream == null || stream.CanRead == false)
                return default(T);

            using (var streamReader = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                var jsonSerializer = new JsonSerializer();
                var result = jsonSerializer.Deserialize<T>(jsonTextReader);
                return result;
            }
        }
    }
}
