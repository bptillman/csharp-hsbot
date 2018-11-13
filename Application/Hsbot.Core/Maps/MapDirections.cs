namespace Hsbot.Core.Maps
{
    public class MapDirections
    {
        public string MapUrl { get; }
        public string Instructions { get; }

        public MapDirections(string mapUrl, string instructions)
        {
            MapUrl = mapUrl;
            Instructions = instructions;
        }
    }
}
