﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hsbot.Core.MessageHandlers.Helpers;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public class CatBombHandler : MessageHandlerBase
    {
        public CatBombHandler(IRandomNumberGenerator randomNumberGenerator) : base(randomNumberGenerator)
        {
        }

        public static Regex CatBombRegex = new Regex(@"cat bomb (\d+)", RegexOptions.Compiled);

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            yield return new MessageHandlerDescriptor {Command = "cat me", Description = "Receive a cat with its tongue out" };
            yield return new MessageHandlerDescriptor { Command = "cat bomb <N>", Description = "get N cats with their tongues out" };
        }

        protected override bool CanHandle(InboundMessage message)
        {
            return message.StartsWith("cat me") || message.StartsWith("cat bomb");
        }

        public override async Task HandleAsync(InboundMessage message)
        {
            var numberOfCats = 5;
            if (message.StartsWith("cat me")) numberOfCats = 1;
            else
            {
                var match = message.Match(CatBombRegex);
                if (match.Success)
                {
                    numberOfCats = match.Groups[1].CaptureToInt() ?? 5;
                }
            }

            var photos = await TumblrApiClient.GetPhotos("tongueoutcats.tumblr.com");
            for (var i = 0; i < numberOfCats; i++)
            {
                var photo = RandomNumberGenerator.GetRandomValue(photos);
                await SendMessage(message.CreateResponse(photo.Url));
            }
        }
    }
}
