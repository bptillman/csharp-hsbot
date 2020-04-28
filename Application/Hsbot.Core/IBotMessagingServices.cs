using System.Threading.Tasks;
using Hsbot.Core.Messaging;

namespace Hsbot.Core
{
    public interface IBotMessagingServices
    {
        Task<IUser> GetChatUserById(string userId);
        Task SendMessage(OutboundResponse response);
        Task UploadFile(FileUploadResponse response);
    }
}