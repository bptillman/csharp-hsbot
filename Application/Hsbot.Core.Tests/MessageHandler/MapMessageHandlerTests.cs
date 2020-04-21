namespace Hsbot.Core.Tests.MessageHandler
{
    using System.Threading.Tasks;
    using MessageHandlers;
    using Infrastructure;
    using Maps;
    using Shouldly;

    public class MapMessageHandlerTests : MessageHandlerTestBase<MapsMessageHandler>
    {
        protected override string[] MessageTextsThatShouldBeHandled => new[]
        {
            "map me Austin",
            "Map Me NASA, Houston",
            "map me Dallas Airport",
            "map me Headspring, Monterrey",
            "Roadmap map me Headspring, Austin",
            "Satellite map me Headspring, Austin",
            "Terrain map me Headspring, Austin",
            "hybrid map me Headspring, Austin"
        };

        protected override string[] MessageTextsThatShouldNotBeHandled => new[]
        {
            "mapme Austin",
            "map meee Austin",
            "can you map me Austin?",
            "map directions from Austin to Houston",
            "directions from Austin and Houston",
            "show directions from Austin to Houston",
            "global map me Houston",
            "plotted map me Houston",
            "riding directions from Austin to Houston",
            "drive directions from Austin to Houston",
            "dive directions from Austin to Houston",
            "hicking directions from Austin to Houston",
        };

        public async Task ShouldGetResponseWithCorrectMapType()
        {
            var mapTypes = new[]
            {
                MapType.Roadmap.ToString(),
                MapType.Satellite.ToString(),
                MapType.Terrain.ToString(),
                MapType.Hybrid.ToString()
            };

            var handler = GetHandlerInstance();

            foreach (var mapType in mapTypes)
            {
                var response = await handler.TestHandleAsync($"{mapType} map me Austin, Texas");
                response.SentMessages.Count.ShouldBe(2);
                response.SentMessages[0].Text.ShouldBe($"{mapType.ToLower()}__Austin, Texas");

                response.SentMessages.Clear();
            }
        }

        public async Task ShouldRoadmapBeDefaultWhenMapTypeIsNotSpecified()
        {
            var handler = GetHandlerInstance();

            var response = await handler.TestHandleAsync("map me Austin, Texas");
            response.SentMessages.Count.ShouldBe(2);
            response.SentMessages[0].Text.ShouldBe($"{MapType.Roadmap.ToString().ToLower()}__Austin, Texas");
        }

        protected override MapsMessageHandler GetHandlerInstance()
        {
            var rng = new RandomNumberGeneratorFake { NextDoubleValue = 0.0 };
            var maps = new MapProviderFake();
            var instance = new MapsMessageHandler(rng, maps);
            return instance;
        }
    }
}
