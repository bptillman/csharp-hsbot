using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hsbot.Core.ApiClients;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public class NopeBombHandler : MessageHandlerBase
    {
        private readonly ITumblrApiClient _tumblrApiClient;

        public NopeBombHandler(IRandomNumberGenerator randomNumberGenerator, ITumblrApiClient tumblrApiClient) : base(randomNumberGenerator)
        {
            _tumblrApiClient = tumblrApiClient;
        }

        public static Regex NopeRegex = new Regex("n(o)+pe", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            yield return new MessageHandlerDescriptor {Command = "n(o+)pe", Description = "Get nopes (one per 'o')"};
        }

        protected override bool CanHandle(InboundMessage message)
        {
            return message.IsMatch(NopeRegex);
        }

        public override async Task HandleAsync(IInboundMessageContext context)
        {
            var match = context.Message.Match(NopeRegex);
            var nopeCount = Math.Min(match.Groups[1].Captures.Count, 10);

            var photos = await _tumblrApiClient.GetPhotos("nopecards.tumblr.com");
            for (var i = 0; i < nopeCount; i++)
            {
                await context.SendResponse(RandomNumberGenerator.GetRandomValue(photos).Url);
            }
        }
    }
}
