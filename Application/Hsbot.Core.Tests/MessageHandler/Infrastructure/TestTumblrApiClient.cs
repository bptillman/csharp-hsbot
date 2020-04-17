using System.Threading.Tasks;
using Hsbot.Core.ApiClients;

namespace Hsbot.Core.Tests.MessageHandler.Infrastructure
{
    public class TestTumblrApiClient : ITumblrApiClient
    {
        public TumblrPhoto[] Photos { get; set; }

        public Task<TumblrPhoto[]> GetPhotos(string blogUrl)
        {
            return Task.FromResult(Photos);
        }
    }
}
