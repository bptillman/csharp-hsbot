using System.IO;

namespace Hsbot.Core.Messaging
{
    public class FileUploadResponse : ResponseBase
    {
        public Stream FileStream { get; set; }
        public string FileName { get; set; }
    }
}