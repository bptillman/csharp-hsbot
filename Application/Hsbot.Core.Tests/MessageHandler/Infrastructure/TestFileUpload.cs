using System.Text;

namespace Hsbot.Core.Tests.MessageHandler.Infrastructure
{
    public class TestFileUpload
    {
        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }

        public string AsString(Encoding encoding = null) => (encoding ?? Encoding.UTF8).GetString(FileBytes);
    }
}