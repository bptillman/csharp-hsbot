using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hsbot.Core.BotServices;
using Hsbot.Core.Brain;
using Hsbot.Core.Messaging;
using Hsbot.Core.Messaging.Formatting;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public class RememberMessageHandler : MessageHandlerBase
    {
        private readonly IMemoryService _memoryService;
        private readonly IChatMessageTextFormatter _chatMessageTextFormatter;

        public RememberMessageHandler(IRandomNumberGenerator randomNumberGenerator, IMemoryService memoryService, IChatMessageTextFormatter chatMessageTextFormatter) : base(randomNumberGenerator)
        {
            _memoryService = memoryService;
            _chatMessageTextFormatter = chatMessageTextFormatter;
        }

        private readonly Regex _memoryRegex = new Regex(@"(?:what is|rem(?:ember)?)\s+(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _newMemoryRegex = new Regex(@"(.*?)(\s+is\s+([\s\S]*))$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _memorySearchRegex = new Regex(@"([^?]+)\??", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _forgetRegex = new Regex(@"forget\s+(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        const string WhatDoYouRemember = "what do you remember";
        const string WhatAreYourFavoriteMemories = "what are your favorite memories";
        const string RandomMemory = "random memory";

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            yield return new MessageHandlerDescriptor { Command = "what is | remember <key>", Description = "Shows a specific memory" };
            yield return new MessageHandlerDescriptor { Command = "remember <key> is <value>", Description = "Creates a new memory" };
            yield return new MessageHandlerDescriptor { Command = "what do you remember?", Description = "Shows everything the bot remembers" };
            yield return new MessageHandlerDescriptor { Command = "forget <key>", Description = "Removes a memory" };
            yield return new MessageHandlerDescriptor { Command = "what are your favorite memories?", Description = "Lists the most remembered memories" };
            yield return new MessageHandlerDescriptor { Command = "random memory", Description = "Shows a random memory" };
        }

        protected override bool CanHandle(InboundMessage message)
        {
            return message.IsMatch(_memoryRegex)
                   || message.StartsWith(WhatDoYouRemember)
                   || message.IsMatch(_forgetRegex)
                   || message.StartsWith(WhatAreYourFavoriteMemories)
                   || message.StartsWith(RandomMemory);
        }

        public override async Task HandleAsync(IInboundMessageContext context)
        {
            var message = context.Message;

            var forgetMatch = message.Match(_forgetRegex);
            if (forgetMatch.Success)
            {
                var memoryToForget = forgetMatch.Groups[1].Value;

                if (_memoryService.Forget(memoryToForget, out var memory))
                {
                    var responseText = $"Ok, I'll forget that {memoryToForget} is {memory.Value}";
                    await context.SendResponse(responseText);
                }

                else
                {
                    await context.SendResponse("I don't remember that :shrug:");
                }

                return;
            }

            var memoryMatch = message.Match(_memoryRegex);
            if (memoryMatch.Success)
            {
                var words = message.Match(_memoryRegex).Groups[1].Value;
                var newMemoryMatch = _newMemoryRegex.Match(words);
                if (newMemoryMatch.Success)
                {
                    var key = newMemoryMatch.Groups[1].Value;
                    var value = newMemoryMatch.Groups[3].Value;

                    PersistenceState persistenceState;
                    if (_memoryService.HasMemory(key, out var oldMemory))
                    {
                        persistenceState = _memoryService.Remember(key, value);
                        await context.SendResponse($"Ok, {key} is updated from {oldMemory.Value} to {value}");
                    }

                    else
                    {
                        persistenceState = _memoryService.Remember(key, value);
                        await context.SendResponse($"Ok, I'll remember {key} is {value}");
                    }

                    if (persistenceState == PersistenceState.InMemoryOnly)
                    {
                        await context.SendResponse($"{_chatMessageTextFormatter.Bold("Warning:")} my brain is on the fritz, so I won't remember this after a reboot.");
                    }

                    return;
                }

                var searchKeyMatch = _memorySearchRegex.Match(words);
                if (searchKeyMatch.Success)
                {
                    var key = searchKeyMatch.Groups[1].Value;
                    if (_memoryService.GetMemory(key, out var memory))
                    {
                        await context.SendResponse(memory.Value);
                    }

                    else
                    {
                        await context.SendResponse($"I don't remember anything matching `{key}`");
                    }
                }

                return;
            }

            if (_memoryService.Count == 0)
            {
                await context.SendResponse("I don't remember anything :shrug:");
                return;
            }

            if (message.StartsWith(WhatDoYouRemember))
            {
                await context.SendResponse($"I remember:\r\n{string.Join("\r\n", _memoryService.GetAllMemories().OrderBy(m => m.Key).Select(m => m.Key))}");
            }

            else if (message.StartsWith(WhatAreYourFavoriteMemories))
            {
                var topMemoryKeys = _memoryService.GetAllMemories()
                    .OrderByDescending(m => m.RememberCount)
                    .Take(5)
                    .Select(m => m.Key);

                var responseText = $"My favorite memories are:\r\n{string.Join("\r\n", topMemoryKeys)}";
                await context.SendResponse(responseText);
            }

            else if (message.StartsWith(RandomMemory))
            {
                var memory = RandomNumberGenerator.GetRandomValue(_memoryService.GetAllMemories().OrderBy(m => m.Key).ToArray());
                await context.SendResponse($"{memory.Key}\r\n{memory.Value}");
            }
        }
    }
}
