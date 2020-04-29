using System.Threading.Tasks;
using Hsbot.Core.ApiClients;

namespace Hsbot.Core.Tests.MessageHandler.Infrastructure
{
    public class TestXkcdApiClient : IXkcdApiClient
    {
        public XkcdInfo InfoToReturn { get; set; } = new XkcdInfo();

        public Task<XkcdInfo> GetInfo(string id = null)
        {
            if (id != null)
            {
                InfoToReturn.Num = id;
                InfoToReturn.Title += $" [{id}]";
            }

            return Task.FromResult(InfoToReturn);
        }
    }
}
