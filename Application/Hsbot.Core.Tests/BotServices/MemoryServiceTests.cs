using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Core.BotServices;
using Hsbot.Core.Tests.Brain;
using Shouldly;
using static Hsbot.Core.Tests.ServiceMocks;

namespace Hsbot.Core.Tests.BotServices
{
    public class MemoryServiceTests
    {
        public async Task ShouldLoadFromBrainOnStart()
        {
            var brain = new FakeBrain();
            var memories = new Dictionary<string, Memory>{{"foo", new Memory { Key = "foo", Value = "bar", RememberCount = 1}}};
            brain.SetItem(MemoryService.MemoriesBrainStorageKey, memories);

            var memoryService = new MemoryService(brain);
            await memoryService.Start(new BotServiceContext());

            memoryService.HasMemory("foo", out var memory).ShouldBeTrue();
            memory.Value.ShouldBe("bar");
        }

        public async Task ShouldUpdateMemoryCountOnCallToGetMemory()
        {
            var brain = new FakeBrain();
            var memories = new Dictionary<string, Memory> { { "foo", new Memory { Key = "foo", Value = "bar", RememberCount = 1 } } };
            brain.SetItem(MemoryService.MemoriesBrainStorageKey, memories);

            var memoryService = new MemoryService(brain);
            await memoryService.Start(new BotServiceContext());

            memoryService.GetMemory("foo", out var memory);
            memory.RememberCount.ShouldBe(2);

            var inBrainMemories = brain.GetItem<Dictionary<string, Memory>>(MemoryService.MemoriesBrainStorageKey);
            inBrainMemories.ContainsKey("foo").ShouldBeTrue();
            inBrainMemories["foo"].RememberCount.ShouldBe(2);
        }

        public async Task ShouldNotUpdateMemoryCountOnCallToHasMemory()
        {
            var brain = new FakeBrain();
            var memories = new Dictionary<string, Memory> { { "foo", new Memory { Key = "foo", Value = "bar", RememberCount = 1 } } };
            brain.SetItem(MemoryService.MemoriesBrainStorageKey, memories);

            var memoryService = new MemoryService(brain);
            await memoryService.Start(new BotServiceContext());

            memoryService.HasMemory("foo", out var memory).ShouldBeTrue();
            memory.RememberCount.ShouldBe(1);

            var inBrainMemories = brain.GetItem<Dictionary<string, Memory>>(MemoryService.MemoriesBrainStorageKey);
            inBrainMemories.ContainsKey("foo").ShouldBeTrue();
            inBrainMemories["foo"].RememberCount.ShouldBe(1);
        }

        public async Task ShouldForget()
        {
            var brain = new FakeBrain();
            var memories = new Dictionary<string, Memory> { { "foo", new Memory { Key = "foo", Value = "bar", RememberCount = 1 } } };
            brain.SetItem(MemoryService.MemoriesBrainStorageKey, memories);

            var memoryService = new MemoryService(brain);
            await memoryService.Start(new BotServiceContext());

            memoryService.Forget("foo", out var memory).ShouldBeTrue();
            memory.RememberCount.ShouldBe(1);

            var inBrainMemories = brain.GetItem<Dictionary<string, Memory>>(MemoryService.MemoriesBrainStorageKey);
            inBrainMemories.ContainsKey("foo").ShouldBeFalse();
        }

        public async Task ShouldRemember()
        {
            var brain = new FakeBrain();
            var memories = new Dictionary<string, Memory>();
            brain.SetItem(MemoryService.MemoriesBrainStorageKey, memories);

            var memoryService = new MemoryService(brain);
            await memoryService.Start(new BotServiceContext());

            var state = memoryService.Remember("foo", "bar");
            state.ShouldBe(brain.PersistenceState);

            memoryService.HasMemory("foo", out var memory).ShouldBeTrue();
            memory.Key.ShouldBe("foo");
            memory.Value.ShouldBe("bar");
            memory.RememberCount.ShouldBe(0);

            var inBrainMemories = brain.GetItem<Dictionary<string, Memory>>(MemoryService.MemoriesBrainStorageKey);
            inBrainMemories.ContainsKey("foo").ShouldBeTrue();
            inBrainMemories["foo"].Value.ShouldBe("bar");
            inBrainMemories["foo"].RememberCount.ShouldBe(0);
        }

        public void ShouldStartupAfterBrainService()
        {
            var memoryService = new MemoryService(new FakeBrain());
            var brainService = new HsbotBrainService(MockBrainStorage().Object, MockLog().Object);

            memoryService.GetStartupOrder().ShouldBeGreaterThan(brainService.GetStartupOrder());
        }
    }
}
