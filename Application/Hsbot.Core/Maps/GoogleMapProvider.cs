namespace Hsbot.Core.Maps
{
    using System.Web;

    public class GoogleMapProvider : IMapProvider
    {
        private const string MapUrl = "http://maps.google.com/maps?t=m&z=11&hl=en&sll=37.0625,-95.677068&sspn=73.579623,100.371094&vpsrc=0";
        private const string ImageMapUrl = "http://maps.google.com/maps/api/staticmap?format=png&sensor=false&size=400x400";

        public Map GetMap(string location, MapType mapType)
        {
            location = HttpUtility.UrlEncode(location);

            var mapUrl = $"{MapUrl}&q={location}&hnear={location}";
            var imageUrl = $"{ImageMapUrl}&markers={location}&maptype={mapType.ToString().ToLower()}";

            return new Map(mapUrl, imageUrl);
        }
    }
}