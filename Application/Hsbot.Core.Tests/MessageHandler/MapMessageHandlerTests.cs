namespace Hsbot.Core.Tests.MessageHandler
{
    using System;
    using System.Threading.Tasks;
    using MessageHandlers;
    using Infrastructure;
    using Maps;
    using Shouldly;

    public class MapMessageHandlerTests : MessageHandlerTestBase<MapsMesssageHandler>
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
            "hybrid map me Headspring, Austin",
            "directions from Austin to Houston",
            "directions from Monterrey, Mexico to Dallas, Texas",
            "Directions From Headspring, Austin to nearest Rudy's",
            "driving directions From Headspring, Austin to nearest Rudy's",
            "walking directions from Monterrey, Mexico to Dallas, Texas",
            "BiCycling Directions From Headspring, Austin to nearest Rudy's",
            "bike directions from Austin to Houston",
            "Biking directions from Austin to Houston"
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

        public async Task ShouldHandlerObtainQueryParametersFromTextCommand()
        {
            var expectedResults = new[]
            {
                "roadmap__Austin",
                "roadmap__NASA, Houston",
                "roadmap__Dallas Airport",
                "roadmap__Headspring, Monterrey",
                "roadmap__Headspring, Austin",
                "satellite__Headspring, Austin",
                "terrain__Headspring, Austin",
                "hybrid__Headspring, Austin",
                "driving__Austin__Houston",
                "driving__Monterrey, Mexico__Dallas, Texas",
                "driving__Headspring, Austin__nearest Rudy's",
                "driving__Headspring, Austin__nearest Rudy's",
                "walking__Monterrey, Mexico__Dallas, Texas",
                "bicycling__Headspring, Austin__nearest Rudy's",
                "bike__Austin__Houston",
                "biking__Austin__Houston"
            };

            MessageTextsThatShouldBeHandled.Length.ShouldBe(expectedResults.Length);

            var messageHandler = GetHandlerInstance();

            for (var i = 0; i < MessageTextsThatShouldBeHandled.Length; i++)
            {
                var response = await messageHandler.TestHandleAsync(MessageTextsThatShouldBeHandled[i]);

                response.SentMessages.Count.ShouldBe(MessageTextsThatShouldBeHandled[i]
                    .Contains("directions from", StringComparison.OrdinalIgnoreCase)
                    ? 3
                    : 2);
                response.SentMessages[1].Text.ShouldBe(expectedResults[i]);

                response.SentMessages.Clear();
            }
        }

        public async Task ShouldHandlerWarnWhenDirectionsAreTheSame()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.TestHandleAsync("directions from Monterrey to Monterrey");

            response.SentMessages.Count.ShouldBe(2);
            response.SentMessages[0].IndicateTyping.ShouldBe(true);
            response.SentMessages[1].Text.ShouldBe("Now you're just being silly.");
        }

        public async Task ShouldHandlerWarnWhenKeyIsEmpty()
        {
            var maps = new MapsFake();
            var messageHandler = GetHandlerInstance(maps);

            maps.FakeKey = null;
            var response = await messageHandler.TestHandleAsync("directions from NASA to Galveston");
            response.SentMessages.Count.ShouldBe(2);
            response.SentMessages[0].IndicateTyping.ShouldBe(true);
            response.SentMessages[1].Text.ShouldBe("Please enter your Google API key in HsbotConfig google:apiKey.");
            response.SentMessages.Clear();

            maps.FakeKey = "";
            response = await messageHandler.TestHandleAsync("directions from NASA to Galveston");
            response.SentMessages.Count.ShouldBe(2);
            response.SentMessages[0].IndicateTyping.ShouldBe(true);
            response.SentMessages[1].Text.ShouldBe("Please enter your Google API key in HsbotConfig google:apiKey.");
        }

        protected override MapsMesssageHandler GetHandlerInstance()
        {
            var maps = new MapsFake();
            return GetHandlerInstance(maps);
        }

        private static MapsMesssageHandler GetHandlerInstance(IMaps maps)
        {
            var rng = new RandomNumberGeneratorFake { NextDoubleValue = 0.0 };
            var instance = new MapsMesssageHandler(rng, maps);
            instance.BotProvidedServices = new BotProvidedServicesFake();
            return instance;
        }
    }
}
