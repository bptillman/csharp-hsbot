using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hsbot.Core.Brain;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hsbot.Core.MessageHandlers
{
    public class StorageMessageHandler : MessageHandlerBase
    {
        private readonly IBotBrain _brain;

        public StorageMessageHandler(IRandomNumberGenerator randomNumberGenerator, IBotBrain brain) : base(randomNumberGenerator)
        {
            _brain = brain;
        }

        public const string ShowStorageCommand = "show storage";
        public const string ShowUsersCommand = "show users";

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            yield return new MessageHandlerDescriptor {Command = "show storage", Description = "Displays contents of the brain"};
            yield return new MessageHandlerDescriptor {Command = "show users", Description = "Displays all users the bot knows about"};

        }

        protected override bool CanHandle(InboundMessage message)
        {
            return message.Is(ShowStorageCommand) || message.Is(ShowUsersCommand);
        }

        public override async Task HandleAsync(IInboundMessageContext context)
        {
            if (context.Message.Is(ShowStorageCommand))
            {
                var brainContents = SerializeBrainToJson();
                await context.UploadFile(brainContents, $"HsBotBrainDump-{DateTime.Now.ToFileTime()}.json");
            }

            if (context.Message.Is(ShowUsersCommand))
            {
                var users = (await context.GetAllUsers()).OrderBy(u => u.FullName).ThenBy(u => u.Email);
                var response = new StringBuilder();
                response.AppendLine("Id,Full Name,Email,Employee");

                foreach (var user in users)
                {
                    response.AppendLine($"{user.Id},{user.FullName},{user.Email},{user.IsEmployee}");
                }

                await context.UploadFile(response.ToString(), $"HsBotUserCache-{DateTime.Now.ToFileTime()}.csv");
            }
        }

        private string SerializeBrainToJson()
        {
            var brainObject = new JObject();

            foreach (var key in _brain.Keys)
            {
                brainObject.Add(new JProperty(key, _brain.GetItem<object>(key)));
            }

            return JsonConvert.SerializeObject(brainObject, Formatting.Indented);
        }
    }
}
