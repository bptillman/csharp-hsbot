namespace Hsbot.Core.MessageHandlers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web;
    using Messaging;
    using Newtonsoft.Json;
    using Random;

    public class SimpsonMessageHandler : MessageHandlerBase
    {
        private const string SimpsonMeCommand = "simpson me";
        private const string SimpsonGifMeCommand = "simpson gif me";
        private const string FrinkiacUrl = "https://frinkiac.com/api/search?q=";

        private static readonly Regex SimpsonMeRegex = new Regex("^(simpson me) (.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex SimpsonGifMeRegex = new Regex("^(simpson gif me) (.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex SimpsonMeWithMemeRegex = new Regex("^(simpson me) (.*) with meme (.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex SimpsonGifMeWithMemeRegex = new Regex("^(simpson gif me) (.*) with meme (.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public SimpsonMessageHandler(IRandomNumberGenerator randomNumberGenerator) : base(randomNumberGenerator)
        {
        }

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            return new List<MessageHandlerDescriptor>
            {
                new MessageHandlerDescriptor { Command = SimpsonMeCommand + " <quote> with meme <caption>", Description = "Get a simpsons image related to the quote you passed it and an optional caption." },
                new MessageHandlerDescriptor { Command = SimpsonGifMeCommand + " <quote> with meme <caption>", Description = "Get a simpsons gif related to the quote you passed it and an optional caption." }
            };
        }

        protected override bool CanHandle(InboundMessage message)
        {
            return message.IsMatch(SimpsonMeRegex) || message.IsMatch(SimpsonGifMeRegex);
        }

        public override async Task HandleAsync(IInboundMessageContext context)
        {
            var message = context.Message;
            var command = GetCommand(message);
            if (command.CommandType == CommandType.None)
                return;

            await context.SendTypingOnChannelResponse();

            var quote = HttpUtility.UrlEncode(command.Quote);
            var requestUrl = $"{FrinkiacUrl}{quote}";

            var reply = $":doh: no {command.GetResourceTypeName()} fit that quote";
            try
            {
                var images = await GetImages(requestUrl);

                if (images.Length > 0)
                {
                    var selectedIndex = images.Length > 10 ? 10 : RandomNumberGenerator.Generate(0, images.Length);
                    var selectedImage = images[selectedIndex];
                    reply = await GetCommandResponse(command, selectedImage);

                    if (!string.IsNullOrEmpty(command.Meme))
                    {
                        reply += $"?b64lines={Base64Encode(command.Meme)}";
                    }
                }
            }
            catch (Exception e)
            {
                reply = e.Message;
            }

            await context.SendResponse(reply);
        }

        private static async Task<FrinkiacImage[]> GetImages(string url)
        {
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
                    return new JsonSerializer().Deserialize<FrinkiacImage[]>(jsonTextReader);
                }
            }
        }

        private static Command GetCommand(InboundMessage message)
        {
            Match match;
            var commandType = CommandType.None;
            string quote = null;
            string meme = null;

            if ((match = message.Match(SimpsonMeRegex)).Success)
            {
                commandType = CommandType.Image;
                quote = match.Groups[2].Value;

                if ((match = message.Match(SimpsonMeWithMemeRegex)).Success)
                {
                    quote = match.Groups[2].Value;
                    meme = match.Groups[3].Value;
                }
            }

            else if ((match = message.Match(SimpsonGifMeRegex)).Success)
            {
                commandType = CommandType.Gif;
                quote = match.Groups[2].Value;

                if ((match = message.Match(SimpsonGifMeWithMemeRegex)).Success)
                {
                    quote = match.Groups[2].Value;
                    meme = match.Groups[3].Value;
                }
            }

            return new Command
            {
                CommandType = commandType,
                Quote = quote,
                Meme = meme
            };
        }

        private static async Task<string> GetCommandResponse(Command command, FrinkiacImage selectedImage)
        {
            switch (command.CommandType)
            {
                case CommandType.Image:
                    return $"https://frinkiac.com/img/{selectedImage.Episode}/{selectedImage.TimeStamp}.jpg";
                case CommandType.Gif:
                    return await GetCommandGifResponse(command, selectedImage);
                default:
                    throw new Exception($"{command.CommandType.ToString()} command type is not supported.");
            }
        }

        private static async Task<string> GetCommandGifResponse(Command command, FrinkiacImage selectedImage)
        {
            var requestUrl = $"https://frinkiac.com/api/frames/{selectedImage.Episode}/{selectedImage.TimeStamp}/5000/5000";
            var images = await GetImages(requestUrl);

            if (images.Length <= 0) return $":doh: no {command.GetResourceTypeName()} fit that quote";

            var gifImagesLenght = images.Length > 42 ? 42 : images.Length;
            var gifImages = images.OrderBy(x => x.TimeStamp).Skip((images.Length / 2) - (gifImagesLenght / 2)).Take(gifImagesLenght).ToArray();
            var startTimeStamp = gifImages.First().TimeStamp;
            var endTimeStamp = gifImages.Last().TimeStamp;

            return $"https://frinkiac.com/gif/{selectedImage.Episode}/{startTimeStamp}/{endTimeStamp}.gif";
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public class Command
        {
            public CommandType CommandType { get; set; }
            public string Quote { get; set; }
            public string Meme { get; set; }

            public string GetResourceTypeName() => $"{CommandType.ToString().ToLower()}s";
        }

        public enum CommandType
        {
            None,
            Image,
            Gif
        }

        public class FrinkiacImage
        {
            public long Id { get; set; }
            public string Episode { get; set; }
            public long TimeStamp { get; set; }
        }
    }
}
