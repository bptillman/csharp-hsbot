namespace Hsbot.Core.Maps
{
    public interface IMapProvider
    {
        Map GetMap(string location, MapType mapType);
    }
}
