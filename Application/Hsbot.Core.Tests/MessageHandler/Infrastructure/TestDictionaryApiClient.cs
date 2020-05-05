using System.Threading.Tasks;
using Hsbot.Core.ApiClients;

namespace Hsbot.Core.Tests.MessageHandler.Infrastructure
{
    public class TestDictionaryApiClient : IDictionaryApiClient
    {
        public DictionaryResponse Response { get; set; }

        public Task<DictionaryResponse> GetDefinition(string searchTerm)
        {
            return Task.FromResult(Response);
        }
    }
}
