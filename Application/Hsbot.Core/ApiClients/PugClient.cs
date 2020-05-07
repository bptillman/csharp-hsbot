using System.Threading.Tasks;

namespace Hsbot.Core.ApiClients
{
    public interface IPugClient
    {
        Task<PugInfo[]> GetPugs(int count = 1);
    }

    public class PugClient : IPugClient
    {
        public Task<PugInfo[]> GetPugs(int count = 1)
        {
            throw new System.NotImplementedException();
        }
    }

    public class PugInfo
    {
        public string Img { get; set; }
    }
}
