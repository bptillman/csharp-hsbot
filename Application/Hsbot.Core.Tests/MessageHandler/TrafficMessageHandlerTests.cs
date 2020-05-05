using System.Linq;
using System.Threading.Tasks;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class TrafficMessageHandlerTests : MessageHandlerTestBase<TrafficMessageHandler>
    {
        protected override string[] MessageTextsThatShouldBeHandled => new[] { "traffic", "traffic hou", "traffic me hou" };
        protected override string[] MessageTextsThatShouldNotBeHandled => new[] { "traf me" };

        public async Task ShouldReturnImageForHoustonTraffic()
        {
            var messageHandler = GetHandlerInstance();

            var response = await messageHandler.TestHandleAsync("traffic hou");

            response.SentMessages.Count.ShouldBe(1);
            response.SentMessages.First().Text.ShouldBe("https://www.google.com/maps/vt/data=orpEIUULaeYEkcdmf1sgEh8zkmnGlbQyKeBJzA_RlLacR4c0jMVo71KkTonslkmmMUQYk9DwhDXkWdCLF325-OE3_PJkgYmsYpib2XC4eZDWw_aqhHPpkONbX-P-NJ1WQZI9oKNqlWtjK_dMvKROi_pFkmDj86wgKCW1jQWaLpKFoXXbwEUPMzeMrFL35Odg_S8");
        }

        public async Task ShouldReturnMessageThatItCannotFindData()
        {
            var messageHandler = GetHandlerInstance();

            var response = await messageHandler.TestHandleAsync("traffic moon");

            response.SentMessages.Count.ShouldBe(1);
            response.SentMessages.First().Text.ShouldBe("Could not find data for moon. I only have data for: aus, hou, dal, mty, gua");
        }
    }
}
