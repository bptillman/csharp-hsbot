using System;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;
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

        public void CommandDescriptorsShouldNotBeNullOrEmpty()
        {
            var messageHandler = GetHandlerInstance();

            var descriptors = messageHandler.GetCommandDescriptors();
            descriptors.ShouldNotBeNull($"Do not return null from {nameof(MessageHandlerBase.GetCommandDescriptors)}. If there's no command for this handler, yield break instead.");

            foreach (var messageHandlerDescriptor in descriptors)
            {
                messageHandlerDescriptor.ShouldNotBeNull($"Do not return null from {nameof(MessageHandlerBase.GetCommandDescriptors)}. If there's no command for this handler, yield break instead.");
                messageHandlerDescriptor.Command.ShouldNotBeEmpty($"{nameof(MessageHandlerDescriptor.Command)} should not be null or empty.");
            }
        }

        protected virtual T GetHandlerInstance(BotProvidedServicesFake botProvidedServices = null)
        {
            //Since this RNG will always return 0, the check on the random roll in the handler will
            //always succeed, meaning the random roll will not cause the result of ShouldHandle
            //to be false
            var rng = new RandomNumberGeneratorFake {NextDoubleValue = 0.0};
            var instance = (T) Activator.CreateInstance(typeof(T), rng);
            instance.BotProvidedServices = botProvidedServices ?? new BotProvidedServicesFake();

            return instance;
        }

        protected virtual T GetHandlerInstance(IRandomNumberGenerator rng, BotProvidedServicesFake botProvidedServices = null)
        {
            var instance = (T) Activator.CreateInstance(typeof(T), rng);
            instance.BotProvidedServices = botProvidedServices ?? new BotProvidedServicesFake();

            return instance;
        }

        protected T GetHandlerInstance(IRandomNumberGenerator rng, IBotProvidedServices botProvidedServices)
        {
            var instance = (T) Activator.CreateInstance(typeof(T), rng);
            instance.BotProvidedServices = botProvidedServices;

            return instance;
        }
    }
}
