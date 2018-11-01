using System;
using Hsbot.Core.MessageHandlers;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler.Infrastructure
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
                handleResult.HandlesMessage.ShouldBeTrue($"{typeof(T).Name}.{nameof(messageHandler.Handles)}(\"{messageText}\") method returned false for a message it should handle");
            }
        }

        public void ShouldNotHandleMessageTexts()
        {
            var messageHandler = GetHandlerInstance();

            foreach (var messageText in MessageTextsThatShouldNotBeHandled)
            {
                var message = messageHandler.GetTestMessageThatWillBeHandled(messageText);

                var handleResult = messageHandler.Handles(message);
                handleResult.HandlesMessage.ShouldBeFalse($"{typeof(T).Name}.{nameof(messageHandler.Handles)}(\"{messageText}\") method returned true for a message it should not handle");
            }
        }

        protected T GetHandlerInstance()
        {
            //Since this RNG will always return 0, the check on the random roll in the handler will
            //always succeed, meaning the random roll will not cause the result of ShouldHandle
            //to be false
            var rng = new RandomNumberGeneratorFake {NextDoubleValue = 0.0}; 

            return (T) Activator.CreateInstance(typeof(T), rng);
        }
    }
}
