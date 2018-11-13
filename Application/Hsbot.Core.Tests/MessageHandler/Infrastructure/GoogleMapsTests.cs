namespace Hsbot.Core.Tests.MessageHandler.Infrastructure
{
    using System;
    using System.Threading.Tasks;
    using Shouldly;
    using System.Web;
    using ApiClient;
    using Maps;

    public class GoogleMapsTests
    {
        private static readonly string[] MapsErrors = new[]
        {
            "Error: No route found.",
            "Error: Service is not working."
        };

        public void ShouldGetMapResponse()
        {
            var maps = GetMapsInstance();
            var route = maps.GetMap("Headspring, Austin", MapType.Roadmap);

            var location = HttpUtility.UrlEncode("Headspring, Austin");

            var endMapUrl = $"&q={location}&hnear={location}";
            route.Url.ShouldEndWith(endMapUrl);

            var endImageUrl = $"&markers={location}&maptype={MapType.Roadmap.ToString().ToLower()}";
            route.ImageUrl.ShouldEndWith(endImageUrl);
        }

        public async Task ShouldGetDirectionsThrowErrorOnTesting()
        {
            var maps = GetMapsInstance();
            MapDirections directions = null;

            try
            {
                directions = await maps.GetDirections("Houston", "NASA", DirectionsMode.Driving);
            }
            catch (Exception e)
            {
                e.Message.ShouldBeOneOf(MapsErrors);
            }
            directions.ShouldBe(null);
        }

        public async Task ShouldWarnWhenDirectionsAreTheSame()
        {
            var maps = GetMapsInstance();
            MapDirections directions = null;

            try
            {
                directions = await maps.GetDirections("Austin", "Austin", DirectionsMode.Walking);
            }
            catch (Exception e)
            {
                e.Message.ShouldBe("Now you're just being silly.");
            }
            directions.ShouldBe(null);
        }

        public async Task ShouldHandlerWarnWhenKeyIsEmpty()
        {
            var maps = GetMapsInstance("");
            MapDirections directions = null;

            try
            {
                directions = await maps.GetDirections("Austin", "Houston", DirectionsMode.Driving);
            }
            catch (Exception e)
            {
                e.Message.ShouldBe("Please enter your Google API key in HsbotConfig google:apiKey.");
            }
            directions.ShouldBe(null);

            maps = GetMapsInstance(null);
            try
            {
                directions = await maps.GetDirections("Austin", "Houston", DirectionsMode.Driving);
            }
            catch (Exception e)
            {
                e.Message.ShouldBe("Please enter your Google API key in HsbotConfig google:apiKey.");
            }
            directions.ShouldBe(null);
        }

        private static IMapProvider GetMapsInstance(string hubotGoogleApiKey)
        {
            return new GoogleMaps
            (
                new HsbotConfig { GoogleApiKey = hubotGoogleApiKey },
                new ApiClient()
             );
        }

        private static IMapProvider GetMapsInstance()
        {
            return GetMapsInstance("FakeApiKey");
        }
    }
}
