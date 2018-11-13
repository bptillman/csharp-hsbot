namespace Hsbot.Core.Maps
{
    public class Map
    {
        public string Url { get; }
        public string ImageUrl { get; }

        public Map(string url, string imageUrl)
        {
            Url = url;
            ImageUrl = imageUrl;
        }
    }
}
