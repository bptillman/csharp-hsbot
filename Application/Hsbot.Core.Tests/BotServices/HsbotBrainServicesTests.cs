using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Core.BotServices;
using Hsbot.Core.Brain;
using Hsbot.Core.Tests.Brain;
using Shouldly;

namespace Hsbot.Core.Tests.BotServices
{
    public class HsbotBrainServicesTests
    {
        public async Task ShouldLoadBrainOnStart()
        {
            var brainStorage = new FakeBrainStorage<InMemoryBrain>
            {
                Brain = new InMemoryBrain(new Dictionary<string, string> {{"foo", "\"bar\""}})
            };

            var brainService = new HsbotBrainService(brainStorage, new FakeLogger<HsbotBrainService>());
            await brainService.Start(new BotServiceContext());

            brainService.GetItem<string>("foo").ShouldBe("bar");
        }

        public async Task ShouldSaveBrainWhenItChanges()
        {
            var brainStorage = new FakeBrainStorage<InMemoryBrain>
            {
                Brain = new InMemoryBrain(new Dictionary<string, string> { { "foo", "\"bar\"" } })
            };

            var brainService = new HsbotBrainService(brainStorage, new FakeLogger<HsbotBrainService>());
            await brainService.Start(new BotServiceContext());

            brainService.SetItem("test", "value");

            brainStorage.SavedBrains.Count.ShouldBe(1);
            brainStorage.SavedBrains[0].Keys.Contains("foo").ShouldBeTrue();
            brainStorage.SavedBrains[0].Keys.Contains("test").ShouldBeTrue();
        }

        public async Task ShouldNotSaveBrainIfInitialLoadFailed()
        {
            var brainStorage = new FakeBrainStorage<InMemoryBrain>
            {
                ThrowExceptionOnLoad = true
            };

            var brainService = new HsbotBrainService(brainStorage, new FakeLogger<HsbotBrainService>());
            await brainService.Start(new BotServiceContext());

            brainService.SetItem("test", "value");

            brainStorage.Brain.Keys.Count.ShouldBe(0);
        }

        public async Task SetItemShouldPersistToStorageWhenInitialLoadSucceeds()
        {
            var brainStorage = new FakeBrainStorage<InMemoryBrain>();

            var brainService = new HsbotBrainService(brainStorage, new FakeLogger<HsbotBrainService>());
            await brainService.Start(new BotServiceContext());

            var persistenceState = brainService.SetItem("test", "value");
            persistenceState.ShouldBe(PersistenceState.Persisted);
        }

        public async Task SetItemShouldSaveInMemoryWhenInitialLoadFailed()
        {
            var brainStorage = new FakeBrainStorage<InMemoryBrain>
            {
                Brain = new InMemoryBrain(new Dictionary<string, string> { { "foo", "\"bar\"" } }),
                ThrowExceptionOnLoad = true
            };

            var brainService = new HsbotBrainService(brainStorage, new FakeLogger<HsbotBrainService>());
            await brainService.Start(new BotServiceContext());

            var persistenceState = brainService.SetItem("test", "value");
            persistenceState.ShouldBe(PersistenceState.InMemoryOnly);
        }
    }
}
