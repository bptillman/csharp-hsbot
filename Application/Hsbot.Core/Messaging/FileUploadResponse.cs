using System.IO;
using System.Text;

namespace Hsbot.Core.Messaging
{
    public class FileUploadResponse : ResponseBase
    {
        public Stream FileStream { get; set; }
        public string FileName { get; set; }
    }

    public class FileContentResponse : ResponseBase
    {
        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }

        public string AsString(Encoding encoding = null) => (encoding ?? Encoding.UTF8).GetString(FileBytes);
    }
}