namespace Hsbot.Core.MessageHandlers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Messaging;
    using Newtonsoft.Json;
    using Random;

    public class XkcdMessageHandler : MessageHandlerBase
    {
        private const string CommandText = "xkcd";
        private const string BaseUrl = "https://xkcd.com/";
        private const string JsonTag = "info.0.json";
        private static readonly Regex XkcdLatestRegex = new Regex("^(xkcd)( latest)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex XkcdNumberRegex = new Regex("^(xkcd )(\\d+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex XkcdRandomRegex = new Regex("^(xkcd random)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public XkcdMessageHandler(IRandomNumberGenerator randomNumberGenerator) : base(randomNumberGenerator)
        {

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
                var comic = string.IsNullOrEmpty(command.Id) ? await GetInfo() : await GetInfo(command.Id);

                await context.SendResponse($"{comic.Title}\n{comic.Img}");
                await context.SendResponse($"{comic.Alt}");
            }
        }

        private static async Task<XkcdInfo> GetInfo(string id = null)
        {
            var url = string.IsNullOrEmpty(id) ? $"{BaseUrl}{JsonTag}" : $"{BaseUrl}{id}/{JsonTag}";
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
            {
                var stream = await response.Content.ReadAsStreamAsync();

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Error: Service is not working.");

                if (stream == null || stream.CanRead == false)
                    throw new Exception("Error: Service is not working.");

                using (var streamReader = new StreamReader(stream))
                using (var jsonTextReader = new JsonTextReader(streamReader))
                {
                    return new JsonSerializer().Deserialize<XkcdInfo>(jsonTextReader);
                }
            }
        }

        private async Task<Command> GetCommand(InboundMessage message)
        {
            Match match;
            string id = null;
            var commandType = CommandType.None;

            if (message.Match(XkcdRandomRegex).Success)
            {
                var currentInfo = await GetInfo();
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

        public class XkcdInfo
        {
            public string Num { get; set; }
            public string Title { get; set; }
            public string Img { get; set; }
            public string Alt { get; set; }
        }
    }
}
