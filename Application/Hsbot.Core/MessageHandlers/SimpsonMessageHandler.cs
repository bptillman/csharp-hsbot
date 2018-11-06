namespace Hsbot.Core.MessageHandlers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
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
        private const string FrinkiacUrl = "https://frinkiac.com/api/search?q=";

        private static readonly Regex SimpsonMeRegex = new Regex("^(simpson me) (.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public SimpsonMessageHandler(IRandomNumberGenerator randomNumberGenerator) : base(randomNumberGenerator)
        {
        }

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            return new List<MessageHandlerDescriptor>
            {
                new MessageHandlerDescriptor { Command = SimpsonMeCommand + " <quote>", Description = "Get a simpsons image related to the quote you passed it" }
            };
        }

        protected override bool CanHandle(InboundMessage message)
        {
            return message.IsMatch(SimpsonMeRegex);
        }

        public override async Task HandleAsync(IBotMessageContext context)
        {
            var command = context.Message.Match(SimpsonMeRegex);
            if (!command.Success)
                return;

            var quote = HttpUtility.UrlEncode(command.Groups[2].Value);
            var requestUrl = $"{FrinkiacUrl}{quote}";

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, requestUrl))
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
                    var jsonSerializer = new JsonSerializer();
                    var images = jsonSerializer.Deserialize<FrinkiacImage[]>(jsonTextReader);

                    var message = "(doh) no images fit that quote";
                    if (images.Length > 0)
                    {
                        var selectedImage = images[RandomNumberGenerator.Generate(0, images.Length)];
                        message = $"http://frinkiac.com/img/{selectedImage.Episode}/{selectedImage.TimeStamp}.jpg";
                    }

                    await ReplyToChannel(context, message);
                }
            }
        }

        public class FrinkiacImage
        {
            public string Id { get; set; }
            public string Episode { get; set; }
            public string TimeStamp { get; set; }
        }
    }
}
