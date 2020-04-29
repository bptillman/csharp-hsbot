using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Hsbot.Core.Messaging
{
    public interface IInboundMessageContext
    {
        InboundMessage Message { get; }
        IBotMessagingServices Bot { get; }
    }

    public class InboundMessageContext : IInboundMessageContext
    {
        public InboundMessageContext(InboundMessage message, IBotMessagingServices botMessagingServices)
        {
            Message = message;
            Bot = botMessagingServices;
        }

        public InboundMessage Message { get; }
        public IBotMessagingServices Bot { get; }
    }

    public static class InboundMessageContextExtensions
    {
        public static Task<IUser> GetChatUserById(this IInboundMessageContext context, string userId)
        {
            return context.Bot.GetChatUserById(userId);
        }

        public static Task<IUser[]> GetAllUsers(this IInboundMessageContext context)
        {
            return context.Bot.GetAllUsers();
        }

        public static Task SendResponse(this IInboundMessageContext context, OutboundResponse response)
        {
            return context.Bot.SendMessage(response);
        }

        public static Task SendResponse(this IInboundMessageContext context, string text)
        {
            return context.Bot.SendMessage(context.Message.CreateResponse(text));
        }

        public static Task SendResponse(this IInboundMessageContext context, string text, Attachment attachment)
        {
            return context.Bot.SendMessage(context.Message.CreateResponse(text, attachment));
        }

        public static Task SendTypingOnChannelResponse(this IInboundMessageContext context)
        {
            return context.Bot.SendMessage(context.Message.CreateTypingOnChannelResponse());
        }

        public static Task UploadFile(this IInboundMessageContext context, string fileContents, string fileName)
        {
            var fileBytes = Encoding.UTF8.GetBytes(fileContents);
            return context.UploadFile(fileBytes, fileName);
        }

        public static async Task UploadFile(this IInboundMessageContext context, byte[] fileBytes, string fileName)
        {
            await using var ms = new MemoryStream(fileBytes);
            await context.Bot.UploadFile(context.Message.CreateFileUploadResponse(ms, fileName));
        }
    }
}
