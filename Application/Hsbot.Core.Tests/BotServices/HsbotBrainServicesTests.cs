using System;
using System.Linq;
using System.Threading.Tasks;
using Hsbot.Core.BotServices;
using Hsbot.Core.Brain;
using Hsbot.Core.Messaging;
using Hsbot.Core.Messaging.Formatting;
using Hsbot.Core.Tests.Infrastructure;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using static Hsbot.Core.Tests.ServiceMocks;
using Moq;

namespace Hsbot.Core.Tests.BotServices
{
    public class HsbotBrainServicesTests
    {
        public async Task ShouldLoadBrainOnConnect()
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
    }
}
