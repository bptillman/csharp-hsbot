using System.Threading.Tasks;
using Hsbot.Core.Brain;
using Moq;

namespace Hsbot.Core.Tests
{
    public static class ServiceMocks
    {
        public static Mock<IBotBrainStorage<InMemoryBrain>> MockBrainStorage()
        {
            var brainStorageMock = new Mock<IBotBrainStorage<InMemoryBrain>>();
            brainStorageMock.Setup(x => x.Load()).Returns(Task.FromResult(new InMemoryBrain()));
            brainStorageMock.Setup(x => x.Save(It.IsAny<InMemoryBrain>())).Returns(Task.CompletedTask);

            return brainStorageMock;
        }
    }
}
