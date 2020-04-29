using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;
using Hsbot.Core.ApiClients;

namespace Hsbot.Core.MessageHandlers
{
    public class XkcdMessageHandler : MessageHandlerBase
    {
        private const string CommandText = "xkcd";
        private static readonly Regex XkcdLatestRegex = new Regex("^(xkcd)( latest)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex XkcdNumberRegex = new Regex("^(xkcd )(\\d+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex XkcdRandomRegex = new Regex("^(xkcd random)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly IXkcdApiClient _xkcdApiClient;

        public XkcdMessageHandler(IRandomNumberGenerator randomNumberGenerator, IXkcdApiClient xkcdApiClient) : base(randomNumberGenerator)
        {
            _xkcdApiClient = xkcdApiClient;
        }

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            yield return new MessageHandlerDescriptor { Command = "xkcd [latest]", Description = "Returns the latest XKCD comic; \"latest\" is optional" };
            yield return new MessageHandlerDescriptor { Command = "xkcd <num>", Description = "Returns XKCD comic #<num>" };
            yield return new MessageHandlerDescriptor { Command = "xkcd random", Description = "Returns a random XKCD comic" };
        }

        protected override bool CanHandle(InboundMessage message)
        {
            return message.StartsWith(CommandText);
        }

        public override async Task HandleAsync(IInboundMessageContext context)
        {
            var message = context.Message;

            await context.SendTypingOnChannelResponse();

            var command = await GetCommand(message);

            if (command.CommandType == CommandType.None)
            {
                await context.SendResponse("Sorry, I don't know that one.");
            }

            else
            {
                var comic = string.IsNullOrEmpty(command.Id)
                    ? await _xkcdApiClient.GetInfo()
                    : await _xkcdApiClient.GetInfo(command.Id);

                await context.SendResponse($"{comic.Title}\n{comic.Img}");
                await context.SendResponse($"{comic.Alt}");
            }
        }

        private async Task<Command> GetCommand(InboundMessage message)
        {
            Match match;
            string id = null;
            var commandType = CommandType.None;

            if (message.Match(XkcdRandomRegex).Success)
            {
                var currentInfo = await _xkcdApiClient.GetInfo();
                id = RandomNumberGenerator.Generate(1, Int32.Parse(currentInfo.Num)).ToString();
                commandType = CommandType.Random;
            }

            else if ((match = message.Match(XkcdNumberRegex)).Success)
            {
                id = match.Groups[2].Value;
                commandType = CommandType.Number;
            }

            else if (message.Match(XkcdLatestRegex).Success)
            {
                commandType = CommandType.Latest;
            }

            return new Command
            {
                Id = id,
                CommandType = commandType
            };
        }

        public class Command
        {
            public CommandType CommandType { get; set; }
            public string Id { get; set; }
        }

        public enum CommandType
        {
            None,
            Latest,
            Random,
            Number
        }
    }
}
