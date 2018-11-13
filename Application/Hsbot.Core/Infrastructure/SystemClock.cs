using System;

namespace Hsbot.Core.Infrastructure
{
    public class SystemClock : ISystemClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}