using System.Linq;
using System.Threading.Tasks;

namespace Hsbot.Core.BotServices
{
    public interface IBotService
    {
        int GetStartupOrder();
        Task Start(BotServiceContext context);
        Task Stop();
    }

    public static class BotStartupOrder
    {
        public const int First = 0;

        public static int After(params int[] dependencyStartupOrders)
        {
            if (dependencyStartupOrders == null || dependencyStartupOrders.Length == 0)
                return First;

            return dependencyStartupOrders.Max() + 1;
        }
    }
}
