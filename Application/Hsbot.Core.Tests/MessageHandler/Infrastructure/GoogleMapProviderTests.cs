namespace Hsbot.Core.Tests.MessageHandler.Infrastructure
{
    using Shouldly;
    using System.Web;
    using Maps;

    public class GoogleMapProviderTests
    {
        public void ShouldGetMapResponse()
        {
            var maps = GetMapProviderInstance();
            var route = maps.GetMap("Headspring, Austin", MapType.Roadmap);

            var location = HttpUtility.UrlEncode("Headspring, Austin");

            var endMapUrl = $"&q={location}&hnear={location}";
            route.Url.ShouldEndWith(endMapUrl);

            var endImageUrl = $"&markers={location}&maptype={MapType.Roadmap.ToString().ToLower()}";
            route.ImageUrl.ShouldEndWith(endImageUrl);
        }

        private static IMapProvider GetMapProviderInstance()
        {
            return new GoogleMapProvider();
        }
    }
}
