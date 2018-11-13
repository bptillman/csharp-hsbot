using System;
using Hsbot.Core.Infrastructure;

namespace Hsbot.Core.Tests.Infrastructure
{
    public class TestSystemClock : ISystemClock
    {
        public DateTime UtcNow { get; set; } = DateTime.UtcNow;
    }
}
