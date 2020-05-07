using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hsbot.Core.ApiClients;

namespace Hsbot.Core.Tests.MessageHandler.Infrastructure
{
    public class TestPugClient : IPugClient
    {
        public IEnumerable<PugInfo> Pugs { get; set; } = new List<PugInfo>();
        
        public Task<PugInfo[]> GetPugs(int count = 1)
        {
            return Task.FromResult(Pugs.Take(count).ToArray());
        }
    }
}
