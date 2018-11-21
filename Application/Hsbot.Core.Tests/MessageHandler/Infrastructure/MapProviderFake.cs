namespace Hsbot.Core.Tests.MessageHandler.Infrastructure
{
    using Maps;

    public class MapProviderFake : IMapProvider
    {
        public Map GetMap(string location, MapType mapType)
        {
            var map = $"{mapType.ToString().ToLower()}__{location}";

            return new Map(map, map);
        }
    }
}
