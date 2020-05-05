using System;
using System.Linq;
using System.Threading.Tasks;
using Hsbot.Core.Messaging;
using Microsoft.AspNetCore.SignalR;

namespace Hsbot.Hosting.Web.LocalDebug.Hubs
{
    public sealed class ChatHub : Hub
    {
        private readonly DebugChatConnector _debugChatConnector;
        public const string ReceiveMethodName = "ReceiveMessage";
        public const string ReceiveUploadMethodName = "ReceiveUpload";

        public ChatHub(DebugChatConnector debugChatConnector)
        {
            _debugChatConnector = debugChatConnector;
        }

        public Task SendMessage(string channelName, string userName, string messageText)
        {
            messageText = messageText.Trim();
            var message = new InboundMessage
            {
                RawText = messageText,
                FullText = messageText,
                TextWithoutBotName = GetTextWithoutBotName(messageText),
                UserId = userName,
                Username = userName,
                UserEmail = $"{userName}@foo.com",
                UserChannel = userName,
                Channel = channelName,
                ChannelName = channelName,
                MessageRecipientType = MessageRecipientType.Channel,
                BotName = "hsbot",
                BotId = "@hsbot",
                BotIsMentioned = BotIsMentioned(messageText)
            };

            _debugChatConnector.ReceiveMessage(message);
            return Task.CompletedTask;
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine("Connected to chat hub");
            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception e)
        {
            Console.WriteLine($"Disconnected {e?.Message} {Context.ConnectionId}");
            await base.OnDisconnectedAsync(e);
        }

        private bool BotIsMentioned(string messageText)
        {
            return _debugChatConnector.BotAliases.Any(n => messageText.StartsWith(n));
        }

        private string GetTextWithoutBotName(string messageText)
        {
            var handle = _debugChatConnector.BotAliases.FirstOrDefault(x => messageText.StartsWith(x, StringComparison.OrdinalIgnoreCase));
            return string.IsNullOrEmpty(handle) ? messageText : messageText.Substring(handle.Length).Trim();
        }
    }
}
