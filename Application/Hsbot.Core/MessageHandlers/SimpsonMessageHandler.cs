using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Hsbot.Core.ApiClients;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public class SimpsonMessageHandler : MessageHandlerBase
    {
        private const string SimpsonMeCommand = "simpson me";
        private const string SimpsonGifMeCommand = "simpson gif me";
        private const string FrinkiacUrl = "https://frinkiac.com/api/search?q=";

        private static readonly Regex SimpsonMeRegex = new Regex("^(simpson me) (.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex SimpsonGifMeRegex = new Regex("^(simpson gif me) (.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex SimpsonMeWithMemeRegex = new Regex("^(simpson me) (.*) with meme (.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex SimpsonGifMeWithMemeRegex = new Regex("^(simpson gif me) (.*) with meme (.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly ISimpsonApiClient _simpsonApiClient;

        public SimpsonMessageHandler(IRandomNumberGenerator randomNumberGenerator, ISimpsonApiClient simpsonApiClient) : base(randomNumberGenerator)
        {
            _simpsonApiClient = simpsonApiClient;
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
                var images = await _simpsonApiClient.GetImages(requestUrl);

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

        private static Command GetCommand(InboundMessage message)
        {
            var result = new Command
            {
                CommandType = CommandType.None,
            };

            var imageMatch = message.Match(SimpsonMeRegex);
            if (imageMatch.Success)
            {
                result.CommandType = CommandType.Image;

                result.Quote = imageMatch.Groups[2].Value;

                var withMemeMatch = message.Match(SimpsonMeWithMemeRegex);
                if (withMemeMatch.Success)
                {
                    result.Meme = withMemeMatch.Groups[3].Value;
                }

                return result;
            }

            var gifMatch = message.Match(SimpsonGifMeRegex);
            if (gifMatch.Success)
            {
                result.CommandType = CommandType.Gif;

                result.Quote = gifMatch.Groups[2].Value;

                var withMemeMatch = message.Match(SimpsonGifMeWithMemeRegex);
                if (withMemeMatch.Success)
                {
                    result.Meme = withMemeMatch.Groups[3].Value;
                }
            }

            return result;
        }

        private async Task<string> GetCommandResponse(Command command, FrinkiacImage selectedImage)
        {
            var imgOrMeme = string.IsNullOrWhiteSpace(command.Meme) ? "img" : "meme";
            switch (command.CommandType)
            {
                case CommandType.Image:
                    return $"https://frinkiac.com/{imgOrMeme}/{selectedImage.Episode}/{selectedImage.TimeStamp}.jpg";
                case CommandType.Gif:
                    return await GetCommandGifResponse(command, selectedImage);
                default:
                    throw new Exception($"{command.CommandType.ToString()} command type is not supported.");
            }
        }

        private async Task<string> GetCommandGifResponse(Command command, FrinkiacImage selectedImage)
        {
            var requestUrl = $"https://frinkiac.com/api/frames/{selectedImage.Episode}/{selectedImage.TimeStamp}/5000/5000";
            var images = await _simpsonApiClient.GetImages(requestUrl);

            if (images.Length <= 0) return $":doh: no {command.GetResourceTypeName()} fit that quote";

            var gifImagesLength = images.Length > 42 ? 42 : images.Length;
            var gifImages = images.OrderBy(x => x.TimeStamp).Skip((images.Length / 2) - (gifImagesLength / 2)).Take(gifImagesLength).ToArray();
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
            Gif,
        }
    }
}
