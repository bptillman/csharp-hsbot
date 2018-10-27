using Hsbot.Slack.Core;
using Microsoft.Extensions.Logging;

namespace Hsbot.Slack.Web
{
    public class HsbotLog : IHsbotLog
    {
        private readonly ILogger _logger;

        public HsbotLog(ILogger<HsbotLog> logger)
        {
            _logger = logger;
        }

        public void Info(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        public void Error(string message, params object[] args)
        {
            _logger.LogError(message, args);
        }

        public void Debug(string message, params object[] args)
        {
            _logger.LogDebug(message, args);
        }

        public void Warn(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }
    }
}
