using System.Collections.Generic;
using Hsbot.Core.FoodTrucks;

namespace Hsbot.Core.Tests.MessageHandler.Infrastructure
{
    public class TestFoodTruckProvider : IFoodTruckProvider
    {
        public IEnumerable<Truck> Trucks { get; set; }

        public IEnumerable<Truck> GetTrucks()
        {
            return Trucks;
        }
    }
}
