using System;
using System.Threading.Tasks;
using Hsbot.Core.Messaging;
using Microsoft.AspNetCore.SignalR.Client;

namespace Hsbot.Blazor.Client
{
    public class ChatClient : IAsyncDisposable
    {
        public const string HubUrl = "/LocalDebug/Chat";
        public const string SendMessageMethodName = "SendMessage";
        public const string ReceiveMessageMethodName = "ReceiveMessage";
        public const string ReceiveUploadMethodName = "ReceiveUpload";

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

                _hubConnection.On<OutboundResponse>(ReceiveMessageMethodName, HandleReceiveMessage);
                _hubConnection.On<FileContentResponse>(ReceiveUploadMethodName, HandleReceiveUpload);

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
        public delegate void MessageReceivedEventHandler(object sender, OutboundResponse response);

        private void HandleReceiveMessage(OutboundResponse response)
        {
            MessageReceived?.Invoke(this, response);
        }

        public event FileUploadReceivedEventHandler FileUploadReceived;
        public delegate void FileUploadReceivedEventHandler(object sender, FileContentResponse response);

        private void HandleReceiveUpload(FileContentResponse response)
        {
            FileUploadReceived?.Invoke(this, response);
        }
    }
}
