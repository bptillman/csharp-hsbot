namespace Hsbot.Core.Maps
{
    using System.Threading.Tasks;

    public interface IMapProvider
    {
        Map GetMap(string location, MapType mapType);
        Task<MapDirections> GetDirections(string origin, string destination, DirectionsMode directionsMode);
    }
}
