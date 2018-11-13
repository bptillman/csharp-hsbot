namespace Hsbot.Core.Tests.MessageHandler.Infrastructure
{
    using System;
    using System.Threading.Tasks;
    using Core.Maps;

    public class MapsFake : IMaps
    {
        public string FakeKey { get; set; }

        public MapsFake()
        {
            FakeKey = "fakeApiKey";
        }

        public Map GetMap(string location, MapType mapType)
        {
            var map = $"{mapType.ToString().ToLower()}__{location}";

            return new Map(map, map);
        }

        public Task<MapDirections> GetDirections(string origin, string destination, DirectionsMode directionsMode)
        {
            if (origin.Equals(destination, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Now you're just being silly.");
            }

            if (string.IsNullOrWhiteSpace(FakeKey))
            {
                throw new Exception("Please enter your Google API key in HsbotConfig google:apiKey.");
            }

            var directions = $"{directionsMode.ToString().ToLower()}__{origin}__{destination}";
            var mapDirections = new MapDirections(directions, directions);

            return Task.FromResult(mapDirections);
        }
    }
}
