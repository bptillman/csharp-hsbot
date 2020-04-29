using System;

namespace Hsbot.Core.MessageHandlers
{
    public class MessageHandlerException : Exception
    {
        public MessageHandlerException(string responseToChannel) : base(responseToChannel)
        {
            ResponseToChannel = responseToChannel;
        }

        public MessageHandlerException(string responseToChannel, string diagnosticInfo) : base(diagnosticInfo)
        {
            ResponseToChannel = responseToChannel;
            DiagnosticInfo = diagnosticInfo;
        }

        public string ResponseToChannel { get; }
        public string DiagnosticInfo { get; }
    }
}
