using System;
using System.Threading.Tasks;
using Hsbot.Core.BotServices;
using Hsbot.Core.Brain;
using static Hsbot.Core.Tests.ServiceMocks;
using Moq;
using Shouldly;

namespace Hsbot.Core.Tests.BotServices
{
    public class HsbotBrainServicesTests
    {
        public async Task ShouldLoadBrainOnStart()
        {
            var logMock = MockLog();
            var brainStorageMock = MockBrainStorage();

            var brainService = new HsbotBrainService(brainStorageMock.Object, logMock.Object);
            await brainService.Start(new BotServiceContext());

            brainStorageMock.Verify(x => x.Load(), Times.Once);
        }

        public async Task ShouldSaveBrainWhenItChanges()
        {
            var logMock = MockLog();
            var brainStorageMock = MockBrainStorage();

            var brainService = new HsbotBrainService(brainStorageMock.Object, logMock.Object);
            await brainService.Start(new BotServiceContext());

            brainService.SetItem("test", "value");

            brainStorageMock.Verify(x => x.Save(It.IsAny<HsbotBrain>()), Times.Once);
        }

        public async Task ShouldNotSaveBrainIfInitialLoadFailed()
        {
            var logMock = MockLog();
            var brainStorageMock = MockBrainStorage();
            brainStorageMock.Setup(x => x.Load()).Throws(new Exception());

            var brainService = new HsbotBrainService(brainStorageMock.Object, logMock.Object);
            await brainService.Start(new BotServiceContext());

            brainService.SetItem("test", "value");

            brainStorageMock.Verify(x => x.Save(It.IsAny<HsbotBrain>()), Times.Never);
        }

        public async Task SetItemShouldPersistToStorageWhenInitialLoadSucceeds()
        {
            var logMock = MockLog();
            var brainStorageMock = MockBrainStorage();

            var brainService = new HsbotBrainService(brainStorageMock.Object, logMock.Object);
            await brainService.Start(new BotServiceContext());

            var persistenceState = brainService.SetItem("test", "value");
            persistenceState.ShouldBe(PersistenceState.Persisted);
        }

        public async Task SetItemShouldSaveInMemoryWhenInitialLoadFailed()
        {
            var logMock = MockLog();
            var brainStorageMock = MockBrainStorage();
            brainStorageMock.Setup(x => x.Load()).Throws(new Exception());

            var brainService = new HsbotBrainService(brainStorageMock.Object, logMock.Object);
            await brainService.Start(new BotServiceContext());

            var persistenceState = brainService.SetItem("test", "value");
            persistenceState.ShouldBe(PersistenceState.InMemoryOnly);
        }
    }
}
