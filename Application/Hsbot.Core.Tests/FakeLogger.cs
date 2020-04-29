using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Hsbot.Core.Tests
{
    public class FakeLogger<T> : ILogger<T>
    {
        public List<string> LoggedMessages { get; } = new List<string>();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            LoggedMessages.Add(formatter(state, exception));
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new FakeLogScope();
        }

        public sealed class FakeLogScope : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}