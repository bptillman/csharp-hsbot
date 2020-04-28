﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Hsbot.Core.Messaging;

namespace Hsbot.Core.Tests.MessageHandler.Infrastructure
{
    public class TestInboundMessageContext : IInboundMessageContext
    {
        public List<OutboundResponse> SentMessages { get; } = new List<OutboundResponse>();
        public Dictionary<string, TestFileUpload> FileUploads { get; } = new Dictionary<string, TestFileUpload>();
        public Dictionary<string, TestChatUser> ChatUsers { get; set; } = new Dictionary<string, TestChatUser>();

        public InboundMessage Message { get; set; }
        public IBotMessagingServices Bot { get; }

        public TestInboundMessageContext(InboundMessage message)
        {
            Message = message;
            Bot = new TestBotMessagingServices
            {
                SendMessageFunc = r =>
                {
                    SentMessages.Add(r);
                    return Task.CompletedTask;
                },
                GetChatUserByIdFunc = id => Task.FromResult((IUser)ChatUsers[id]),
                FileUploadFunc = r =>
                {
                    using var ms = new MemoryStream();
                    r.FileStream.CopyTo(ms);

                    FileUploads.Add(r.FileName, new TestFileUpload
                    {
                        FileBytes = ms.ToArray(),
                        FileName = r.FileName
                    });

                    return Task.CompletedTask;
                }
            };
        }

        public class TestFileUpload
        {
            public string FileName { get; set; }
            public byte[] FileBytes { get; set; }

            public string AsString(Encoding encoding = null) => (encoding ?? Encoding.UTF8).GetString(FileBytes);
        }
    }

    public class TestBotMessagingServices : IBotMessagingServices
    {
        public Func<OutboundResponse, Task> SendMessageFunc { get; set; }
        public Func<string, Task<IUser>> GetChatUserByIdFunc { get; set; }
        public Func<FileUploadResponse, Task> FileUploadFunc { get; set; }

        public Task<IUser> GetChatUserById(string userId)
        {
            return GetChatUserByIdFunc(userId);
        }

        public Task SendMessage(OutboundResponse response)
        {
            return SendMessageFunc(response);
        }

        public Task UploadFile(FileUploadResponse response)
        {
            return FileUploadFunc(response);
        }
    }
}
