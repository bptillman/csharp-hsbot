using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hsbot.Core.ApiClients;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public class DefineMessageHandler : MessageHandlerBase
    {
        private readonly Regex regex = new Regex("^define (.+)", RegexOptions.Compiled);

        private readonly IDictionaryApiClient _dictionaryApiClient;

        public DefineMessageHandler(IRandomNumberGenerator randomNumberGenerator, IDictionaryApiClient dictionaryApiClient) : base(randomNumberGenerator)
        {
            _dictionaryApiClient = dictionaryApiClient;
        }

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            yield return new MessageHandlerDescriptor
            {
                Command = "define <word>",
                Description = "define <word>. Returns definition of a word."
            };
        }

        protected override bool CanHandle(InboundMessage message)
        {
            return message.IsMatch(regex);
        }

        public override async Task HandleAsync(IInboundMessageContext context)
        {
            await context.SendTypingOnChannelResponse();

            var match = context.Message.Match(regex);
            var searchTerm = match.Groups[1].Value;
            var response = await _dictionaryApiClient.GetDefinition(searchTerm);

            if (response.HasDefinition)
            {
                await context.SendResponse($"Here are definitions for *{searchTerm}*:\n\t- {string.Join("\n\t- ", response.Definitions)}");
                return;
            }

            if (response.HasRecommendations)
            {
                await context.SendResponse($"Did you mean: {string.Join(", ", response.Recommendations)}");
                return;
            }

            await context.SendResponse($"{searchTerm} means nothing to me");
        }
    }
}
