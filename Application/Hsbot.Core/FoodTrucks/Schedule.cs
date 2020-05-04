using System;
using System.Linq;
using Hsbot.Core.MessageHandlers.Helpers;

namespace Hsbot.Core.FoodTrucks
{
    public class Schedule
    {
        public DayOfWeek[] Days { get; set; }
        public int[] Weeks { get; set; }
        public ScheduleType Type { get; set; }
        public string TypeDescription => Type.ToString().ToLower();
        public string Time { get; set; }
        public bool Disabled { get; set; }
        public bool Enabled => !Disabled;
        public bool Matches(in DateTime today) => Days.Contains(today.DayOfWeek) && Weeks.Contains(today.GetWeekNumberOfMonth());
    }
}
