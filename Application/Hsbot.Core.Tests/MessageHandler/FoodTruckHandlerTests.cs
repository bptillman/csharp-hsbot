using System;
using System.Linq;
using System.Threading.Tasks;
using Hsbot.Core.FoodTrucks;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Messaging;
using Hsbot.Core.Tests.Infrastructure;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class FoodTruckHandlerTests : MessageHandlerTestBase<FoodTruckHandler>
    {
        protected override string[] MessageTextsThatShouldBeHandled => new[] { "foodtruck", "food truck", "foodtruck schedule", "food truck schedule" };
        protected override string[] MessageTextsThatShouldNotBeHandled => new[] { "food", "truck", "schedule" };

        public async Task ShouldReturnAMessageWithTheLink()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.TestHandleAsync("foodtruck schedule");
            response.SentMessages.Count.ShouldBe(1);
            response.SentMessages.First().Text.ShouldBe("Here is the food truck schedule for The Campus: https://raw.githubusercontent.com/HeadspringLabs/hsbot/master/foodtruckschedule.jpg");
        }

        public async Task ShouldReturnAMessageWithFoodTruckInfo()
        {
            var messageHandler = GetHandlerInstance(new DateTime(2010, 1, 4));
            var message = GetMessage(messageHandler, "foodtruck");
            var context = GetMessageContext(message);

            await messageHandler.HandleAsync(context);
            context.SentMessages.Count.ShouldBe(1);
            context.SentMessages.First().Text.ShouldBe("The breakfast food truck for Monday is Joe's Burgers, and they will be here from 1pm to 2pm, which you can verify here: http://www.joesburgers.com\n:chompy:");
        }


        public async Task ShouldReturnAMessageWithNoFoodTruckInfo()
        {
            var messageHandler = GetHandlerInstance(new DateTime(2010, 1, 5));
            var message = GetMessage(messageHandler, "foodtruck");
            var context = GetMessageContext(message);

            await messageHandler.HandleAsync(context);
            context.SentMessages.Count.ShouldBe(1);
            context.SentMessages.First().Text.ShouldBe("No food truck today :sadpanda:");
        }

        public async Task ShouldReturnAMessageWithNextWeekFoodTruckInfo()
        {
            var messageHandler = GetHandlerInstance(new DateTime(2010, 1, 11));
            var message = GetMessage(messageHandler, "foodtruck");
            var context = GetMessageContext(message);

            await messageHandler.HandleAsync(context);
            context.SentMessages.Count.ShouldBe(1);
            context.SentMessages.First().Text.ShouldBe("No food truck today, but next week it will be:\n\tThe breakfast food truck for *NEXT* Monday is Joe's Burgers, and they will be here from 1pm to 2pm, which you can verify here: http://www.joesburgers.com\n:chompy:");
        }

        protected TestInboundMessageContext GetMessageContext(InboundMessage inboundMessage)
        {
            return new TestInboundMessageContext(inboundMessage)
            {
                ChatUsers =
                {
                    {"bob", new TestChatUser {Id = "bob", IsEmployee = true, Email = "bob@bob.com", FullName = "bob"}},
                    {"bot", new TestChatUser {IsEmployee = false}},
                    {"notInJira", new TestChatUser {IsEmployee = true}},
                    {"nobody", new TestChatUser {Id = "nobody", IsEmployee = true, FullName = "nobody"}},
                }
            };
        }

        protected override FoodTruckHandler GetHandlerInstance()=> GetHandlerInstance(DateTime.UtcNow);

        private InboundMessage GetMessage(MessageHandlerBase handler, string messageText)
        {
            var inboundMessage = new InboundMessage
            {
                BotIsMentioned = true,
                BotId = "test",
                BotName = "test",
                Channel = handler.GetTestMessageChannel(),
                ChannelName = handler.GetTestMessageChannel(),
                FullText = messageText,
                MessageRecipientType = MessageRecipientType.Channel,
                RawText = messageText,
                TextWithoutBotName = messageText,
                UserChannel = "",
                UserEmail = "test@test.com",
                UserId = "nobody",
                Username = "nobody"
            };
            return inboundMessage;
        }

        private FoodTruckHandler GetHandlerInstance(DateTime dateTime)
        {
            var rng = new RandomNumberGeneratorFake();
            var systemClock = new TestSystemClock {UtcNow = dateTime};
            var foodTruckProvider = new TestFoodTruckProvider
            {
                Trucks = new[]
                {
                    new Truck
                    {
                        Name = "Joe's Burgers",
                        Site = "http://www.joesburgers.com",
                        Schedules = new[]
                        {
                            new Schedule
                            {
                                Type = ScheduleType.Breakfast,
                                Weeks = new[] {1,3},
                                Days = new[] {DayOfWeek.Monday},
                                Time = "1pm to 2pm"
                            }
                        }
                    }
                }
            };

            var handler = new FoodTruckHandler(rng, foodTruckProvider, systemClock);

            return handler;
        }
    }
}
