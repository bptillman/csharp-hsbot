using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hsbot.Core.ApiClients;

namespace Hsbot.Core.Tests.MessageHandler.Infrastructure
{
    public class TestSimpsonApiClient : ISimpsonApiClient
    {
        public IEnumerable<FrinkiacImage> Images { get; set; } = new List<FrinkiacImage>();

        public Task<FrinkiacImage[]> GetImages(string url)
        {
            return Task.FromResult(Images.ToArray());
        }
    }
}
