using System;

namespace Hsbot.Core.Messaging.Formatting
{
    /// <summary>
    /// Provides a common interface for chat-client-specific formatting on outgoing message text
    /// </summary>
    public interface IChatMessageTextFormatter
    {
        string FormatDate(DateTime date, string format);
    }
}
