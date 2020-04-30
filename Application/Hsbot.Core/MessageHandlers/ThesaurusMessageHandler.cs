using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hsbot.Core.ApiClients;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public class ThesaurusMessageHandler : MessageHandlerBase
    {
        private readonly Regex regex = new Regex("^(lookup|look up|choices for) (.+)", RegexOptions.Compiled);

        private readonly IThesaurusApiClient _thesaurusApiClient;

        public ThesaurusMessageHandler(IRandomNumberGenerator randomNumberGenerator, IThesaurusApiClient thesaurusApiClient) : base(randomNumberGenerator)
        {
            _thesaurusApiClient = thesaurusApiClient;
        }

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            yield return new MessageHandlerDescriptor
            {
                Command = "lookup | look up | choices for <word>",
                Description = "lookup | look up | choices for <word>. Returns synonyms, antonyms and definition of a word."
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
            var searchTerm = match.Groups[2].Value;
            var response = await _thesaurusApiClient.LookUp(searchTerm);

            if (!response.HasSomething)
            {
                await context.SendResponse($"{searchTerm} means nothing to me");
                return;
            }

            await context.SendResponse($"Information for *{searchTerm}*:");

            if (response.HasSynonyms)
            {
                await context.SendResponse($"Synonyms: {string.Join(", ", response.Synonyms)}");
            }

            if (response.HasAntonyms)
            {
                await context.SendResponse($"Antonyms: {string.Join(", ", response.Antonyms)}");
            }

            if (response.HasDefinitions)
            {

                await context.SendResponse($"Definitions:\n\t - {string.Join("\n\t - ", response.Definitions)}");
            }
        }
    }
}
