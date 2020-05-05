using System.Threading.Tasks;
using Hsbot.Core.ApiClients;

namespace Hsbot.Core.Tests.MessageHandler.Infrastructure
{
    public class TestThesaurusApiClient : IThesaurusApiClient
    {
        public ThesaurusResponse Response { get; set; } = new ThesaurusResponse();

        public Task<ThesaurusResponse> LookUp(string searchTerm)
        {
            return Task.FromResult(Response);
        }
    }
}
