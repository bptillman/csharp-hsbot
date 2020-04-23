using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hsbot.Core.Brain;
using Hsbot.Core.Messaging;
using Hsbot.Core.Messaging.Formatting;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public class RememberMessageHandler : MessageHandlerBase
    {
        private readonly IBotBrain _brain;
        private readonly IChatMessageTextFormatter _chatMessageTextFormatter;

        public RememberMessageHandler(IRandomNumberGenerator randomNumberGenerator, IBotBrain brain, IChatMessageTextFormatter chatMessageTextFormatter) : base(randomNumberGenerator)
        {
            _brain = brain;
            _chatMessageTextFormatter = chatMessageTextFormatter;
        }

        public const string MemoriesBrainStorageKey = "remember";
        public const string MemoryCountBrainStorageKey = "memoriesByRecollection";

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
            var memories = LoadMemories();
            var memoryCounts = LoadMemoryCounts();

            var message = context.Message;

            var forgetMatch = message.Match(_forgetRegex);
            if (forgetMatch.Success)
            {
                var memoryToForget = forgetMatch.Groups[1].Value;

                if (memoryCounts.Remove(memoryToForget))
                {
                    SaveMemoryCounts(memoryCounts);
                }

                if (memories.Remove(memoryToForget, out var value))
                {
                    var responseText = $"Ok, I'll forget that {memoryToForget} is {value}";
                    memories.Remove(memoryToForget);

                    await context.SendResponse(responseText);
                    SaveMemories(memories);
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

                    if (memories.TryGetValue(key, out var oldValue))
                    {
                        await context.SendResponse($"Ok, {key} is updated from {oldValue} to {value}");
                    }

                    else
                    {
                        await context.SendResponse($"Ok, I'll remember {key} is {value}");
                    }

                    memories[key] = value;
                    if (SaveMemories(memories) == PersistenceState.InMemoryOnly)
                    {
                        await context.SendResponse($"{_chatMessageTextFormatter.Bold("Warning:")} my brain is on the fritz, so I won't remember this after a reboot.");
                    }

                    return;
                }

                var searchKeyMatch = _memorySearchRegex.Match(words);
                if (searchKeyMatch.Success)
                {
                    var key = searchKeyMatch.Groups[1].Value;
                    if (memories.TryGetValue(key, out var value))
                    {
                        var counter = 1;
                        if (memoryCounts.ContainsKey(key))
                        {
                            counter = memoryCounts[key] + 1;
                        }

                        memoryCounts[key] = counter;
                        SaveMemoryCounts(memoryCounts);

                        await context.SendResponse(value);
                    }

                    else
                    {
                        await context.SendResponse($"I don't remember anything matching `{key}`");
                    }
                }

                return;
            }

            if (memories.Count == 0)
            {
                await context.SendResponse("I don't remember anything :shrug:");
                return;
            }

            if (message.StartsWith(WhatDoYouRemember))
            {
                await context.SendResponse($"I remember:\r\n{string.Join("\r\n", memories.Keys)}");
            }

            else if (message.StartsWith(WhatAreYourFavoriteMemories))
            {
                var topMemoryKeys = memories.Keys
                    .OrderByDescending(k => memoryCounts.ContainsKey(k) ? memoryCounts[k] : 0)
                    .Take(5);

                var responseText = $"My favorite memories are:\r\n{string.Join("\r\n", topMemoryKeys)}";
                await context.SendResponse(responseText);
            }

            else if (message.StartsWith(RandomMemory))
            {
                var memory = RandomNumberGenerator.GetRandomValue(memories);
                await context.SendResponse($"{memory.Key}\r\n{memory.Value}");
            }
        }

        private Dictionary<string, int> LoadMemoryCounts()
        {
            return _brain.GetItem<Dictionary<string, int>>(MemoryCountBrainStorageKey).ToCaseInsensitiveDictionary();
        }

        private Dictionary<string, string> LoadMemories()
        {
            return _brain.GetItem<Dictionary<string, string>>(MemoriesBrainStorageKey).ToCaseInsensitiveDictionary();
        }

        private void SaveMemoryCounts(Dictionary<string, int> memoryCounts)
        {
            _brain.SetItem(MemoryCountBrainStorageKey, memoryCounts);
        }

        private PersistenceState SaveMemories(Dictionary<string, string> memories)
        {
            return _brain.SetItem(MemoriesBrainStorageKey, memories);
        }
    }
}
