using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace Hsbot.Blazor.Client
{
    public class ChatClient : IAsyncDisposable
    {
        public const string HubUrl = "/LocalDebug/Chat";
        public const string SendMessageMethodName = "SendMessage";
        public const string ReceiveMessageMethodName = "ReceiveMessage";

        private bool _started = false;

        private readonly string _hubUrl;
        private HubConnection _hubConnection;
        
        public ChatClient(string serverUrl)
        {
            _hubUrl = serverUrl.TrimEnd('/') + HubUrl;
        }

        public async Task StartAsync()
        {
            
            if (!_started)
            {
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(_hubUrl)
                    .Build();

                Console.WriteLine("ChatClient: calling Start()");

                _hubConnection.On<string, string, string, bool>(ReceiveMessageMethodName, HandleReceiveMessage);

                await _hubConnection.StartAsync();

                Console.WriteLine("ChatClient: Start returned");
                _started = true;
            }
        }

        public async Task StopAsync()
        {
            if (_started)
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
                _started = false;
            }
        }

        public async Task SendAsync(string channelName, string userName, string messageText)
        {
            if (!_started)
                throw new InvalidOperationException("Client not started");

            await _hubConnection.SendAsync(SendMessageMethodName, channelName, userName, messageText);
        }
        
        public async ValueTask DisposeAsync()
        {
            Console.WriteLine("ChatClient: Disposing");
            await StopAsync();
        }

        public event MessageReceivedEventHandler MessageReceived;
        public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);

        private void HandleReceiveMessage(string channelName, string userName, string messageText, bool indicateTyping)
        {
            var messageReceivedEvent = new MessageReceivedEventArgs
            {
                ChannelName = channelName,
                IndicateTyping = indicateTyping,
                MessageText = messageText,
                UserName = userName
            };

            MessageReceived?.Invoke(this, messageReceivedEvent);
        }

        public class MessageReceivedEventArgs : EventArgs
        {
            public string ChannelName { get; set; }
            public string UserName { get; set; }
            public string MessageText { get; set; }
            public bool IndicateTyping { get; set; }
        }
    }
}
