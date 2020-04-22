using System.Threading.Tasks;

namespace Hsbot.Core.BotServices
{
    public interface IBotService
    {
        int StartupOrder { get; }
        Task Start(BotServiceContext context);
        Task Stop();
    }
}
