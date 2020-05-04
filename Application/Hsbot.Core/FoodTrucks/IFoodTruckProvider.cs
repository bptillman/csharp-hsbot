using System.Collections.Generic;

namespace Hsbot.Core.FoodTrucks
{
    public interface IFoodTruckProvider
    {
        IEnumerable<Truck> GetTrucks();
    }
}
