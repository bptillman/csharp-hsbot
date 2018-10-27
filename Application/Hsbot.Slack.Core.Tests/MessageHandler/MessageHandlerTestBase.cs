using System;
using Hsbot.Slack.Core.MessageHandlers;
using Shouldly;

namespace Hsbot.Slack.Core.Tests.MessageHandler
{
    public abstract class MessageHandlerTestBase<T> where T: MessageHandlerBase
    {
        protected virtual string[] MessageTextsThatShouldBeHandled => new string[] { };
        protected virtual string[] MessageTextsThatShouldNotBeHandled => new string[] { };

        public void ShouldHandleMessageTexts()
        {
            var messageHandler = GetHandlerInstance();

            foreach (var messageText in MessageTextsThatShouldBeHandled)
            {
                var message = messageHandler.GetTestMessageThatWillBeHandled(messageText);

                var handleResult = messageHandler.Handles(message);
                handleResult.HandlesMessage.ShouldBeTrue();
            }
        }

        public void ShouldNotHandleMessageTexts()
        {
            var messageHandler = GetHandlerInstance();

            foreach (var messageText in MessageTextsThatShouldNotBeHandled)
            {
                var message = messageHandler.GetTestMessageThatWillBeHandled(messageText);

                var handleResult = messageHandler.Handles(message);
                handleResult.HandlesMessage.ShouldBeFalse();
            }
        }

        protected T GetHandlerInstance()
        {
            //Since this RNG will always return 0, the check on the random roll in the handler will
            //always succeed, meaning the random roll will not prevent the result of ShouldHandle
            //from being false
            var rng = new RandomNumberGeneratorFake {NextDoubleValue = 0.0}; 

            return (T) Activator.CreateInstance(typeof(T), rng);
        }
    }
}
