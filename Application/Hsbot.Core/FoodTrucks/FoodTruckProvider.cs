using System;
using System.Collections.Generic;
using System.Linq;

namespace Hsbot.Core.FoodTrucks
{
    public class FoodTruckProvider : IFoodTruckProvider
    {
        private static readonly int[] OddWeeks = { 1, 3, 5 };
        private static readonly int[] EvenWeeks = { 2, 4, 6 };
        private static readonly int[] AllWeeks = { 1, 2, 3, 4, 5, 6 };

        private readonly IEnumerable<Truck> _trucks = new[]
        {
            new Truck
            {
                Name = "Paprika",
                Site = "the internet",
                Schedules = new[]
                {
                    new Schedule
                    {
                        Type = ScheduleType.Lunch,
                        Days = new[] {DayOfWeek.Monday},
                        Weeks = OddWeeks,
                        Time = "11:30am - 1:30pm"
                    }
                }
            },
            new Truck
            {
                Name = "Mission Hot Dogs",
                Site = "https://twitter.com/MissionHotDogs",
                Schedules = new[]
                {
                    new Schedule
                    {
                        Type = ScheduleType.Lunch,
                        Days = new[] {DayOfWeek.Monday},
                        Weeks = EvenWeeks,
                        Time = "11:00am - 2:00pm"
                    }
                }
            },
            new Truck
            {
                Name = "Top Taco",
                Site = "http://www.tacofoodgroup.com/",
                Schedules = new[]
                {

                    new Schedule
                    {
                        Type = ScheduleType.Breakfast,
                        Days = new[] {DayOfWeek.Tuesday},
                        Weeks = EvenWeeks,
                        Time = "8:00am - 10:15am"
                    },
                    new Schedule
                    {
                        Type = ScheduleType.Breakfast,
                        Days = new[] {DayOfWeek.Thursday},
                        Weeks = OddWeeks,
                        Time = "8:00am - 10:15am"
                    }
                }
            },
            new Truck
            {
                Name = "Cafe Ybor",
                Site = "http://www.cafeybor.com",
                Schedules = new[]
                {
                    new Schedule
                    {
                        Type = ScheduleType.Lunch,
                        Days = new[] {DayOfWeek.Tuesday},
                        Weeks = EvenWeeks,
                        Time = "11:00am - 1:30pm"
                    }
                }
            },
            new Truck
            {
                Name = "Heros Gyros",
                Site = "http://www.theherosgyros.com",
                Schedules = new[]
                {
                    new Schedule
                    {
                        Type = ScheduleType.Lunch,
                        Days = new[] {DayOfWeek.Thursday},
                        Weeks = EvenWeeks,
                        Time = "11:30am - 1:30pm"
                    }
                }
            },
            new Truck
            {
                Name = "Nearby Coffee Co.",
                Site = "the internet",
                Schedules = new[]
                {
                    new Schedule
                    {
                        Type = ScheduleType.Other,
                        Days = new[] {DayOfWeek.Monday},
                        Weeks = AllWeeks,
                        Time = "3:10pm - 3:45pm"
                    },
                    new Schedule
                    {
                        Type = ScheduleType.Other,
                        Days = new[] {DayOfWeek.Tuesday},
                        Weeks = AllWeeks,
                        Time = "2:00pm - 2:45pm"
                    }
                }
            }
        };

        public IEnumerable<Truck> GetTrucks() => _trucks.Where(x => x.Enabled);
    }
}
