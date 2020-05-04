using System;

namespace Hsbot.Core.Infrastructure
{
    public interface ISystemClock
    {
        DateTime UtcNow { get; }
        DateTime LocalTimeNow(TimeSpan offset);
    }
}
