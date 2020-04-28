using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Hsbot.Core.FoodTrucks;
using Hsbot.Core.Infrastructure;
using Hsbot.Core.MessageHandlers.Helpers;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public class FoodTruckHandler : MessageHandlerBase
    {
        private const string _scheduleUrl = "https://raw.githubusercontent.com/HeadspringLabs/hsbot/master/foodtruckschedule.jpg";

        private readonly IFoodTruckProvider _foodTruckProvider;
        private readonly ISystemClock _systemClock;

        public FoodTruckHandler(IRandomNumberGenerator randomNumberGenerator, IFoodTruckProvider foodTruckProvider, ISystemClock systemClock) : base(randomNumberGenerator)
        {
            _foodTruckProvider = foodTruckProvider;
            _systemClock = systemClock;
        }

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            yield return new MessageHandlerDescriptor {Command = "food truck|foodtruck", Description = "Gets information on food trucks at the office." };
            yield return new MessageHandlerDescriptor { Command = "food truck|foodtruck schedule", Description = "Gets information on food trucks at the office." };
        }

        protected override bool CanHandle(InboundMessage message)
        {
            return message.StartsWith("food truck") || message.StartsWith("foodtruck");
        }

        public override async Task HandleAsync(IInboundMessageContext context)
        {
            if (context.Message.EndsWith("schedule"))
            {
                await context.SendResponse($"Here is the food truck schedule for The Campus: {_scheduleUrl}");
                return;
            }

            var user = await context.GetChatUserById(context.Message.UserId);
            var today = _systemClock.LocalTimeNow(user.TimeZoneOffset);

            if (today.Hour > 20)
            {
                today = today.AddDays(1);
            }

            var schedules = new List<string>();
            var nextWeekSchedules = new List<string>();

            foreach (var truck in _foodTruckProvider.GetTrucks())
            {
                foreach (var schedule in truck.Schedules.Where(s => s.Enabled))
                {
                    if (schedule.Matches(today))
                    {
                        schedules.Add($"The {schedule.TypeDescription} food truck for {today.DayOfWeek} is {truck.Name}, and they will be here from {schedule.Time}, which you can verify here: {truck.Site}");
                    }
                    else if (schedule.Matches(today.AddDays(7)))
                    {
                        nextWeekSchedules.Add($"The {schedule.TypeDescription} food truck for *NEXT* {today.DayOfWeek} is {truck.Name}, and they will be here from {schedule.Time}, which you can verify here: {truck.Site}");
                    }
                }
            }

            if (schedules.Any())
            {
                await context.SendResponse($"{string.Join("\n", schedules)}\n:chompy:");
                return;
            }

            if (nextWeekSchedules.Any())
            {
                await context.SendResponse($"No food truck today, but next week it will be:\n\t{string.Join("\n\t", nextWeekSchedules)}\n:chompy:");
                return;
            }

            await context.SendResponse("No food truck today :sadpanda:");
        }
    }
}
