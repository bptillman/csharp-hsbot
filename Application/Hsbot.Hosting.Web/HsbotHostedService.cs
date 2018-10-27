using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Hsbot.Hosting.Web
{
    public class HsbotHostedService : IHostedService
    {
      private readonly Slack.Core.Hsbot _hsbot;

      public HsbotHostedService(Slack.Core.Hsbot hsbot)
      {
        _hsbot = hsbot;
      }

      public Task StartAsync(CancellationToken cancellationToken)
      {
          return _hsbot.Connect();
      }

      public Task StopAsync(CancellationToken cancellationToken)
      {
        return _hsbot.Disconnect();
      }
    }
}
