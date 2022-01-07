namespace Hsbot.Hosting.Web
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class HsbotHostedService : IHostedService
    {
      private readonly Core.Hsbot _hsbot;
      private readonly ILogger<HsbotHostedService> _logger;

      public HsbotHostedService(Core.Hsbot hsbot, ILogger<HsbotHostedService> logger)
      {
          _hsbot = hsbot;
          _logger = logger;
      }

      public Task StartAsync(CancellationToken cancellationToken)
      {
          try
          {
              return _hsbot.Connect();
          }
          catch (Exception e)
          {
              _logger.LogError(e, "Error starting hosted service");
              throw;
          }
      }

      public Task StopAsync(CancellationToken cancellationToken)
      {
        return _hsbot.Disconnect();
      }
    }
}
