using System.Collections.Generic;

namespace Hsbot.Core.FoodTrucks
{
    public class Truck
    {
        public string Name { get; set; }
        public string Site { get; set; }
        public IEnumerable<Schedule> Schedules { get; set; }
        public bool Disabled { get; set; }
        public bool Enabled => !Disabled;
    }
}
