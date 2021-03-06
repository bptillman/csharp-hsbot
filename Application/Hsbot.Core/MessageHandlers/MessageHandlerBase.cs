using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public abstract class MessageHandlerBase : IInboundMessageHandler
    {
        protected readonly IRandomNumberGenerator RandomNumberGenerator;

        public static readonly string[] AllChannels = null;
        public static readonly string[] FunChannels = { "#general", "#headspring", "#developers", "#austin", "#houston", "#dallas", "#monterrey", "#hsbottesting" };
        public virtual string[] CannedResponses => new string[0];

        protected MessageHandlerBase(IRandomNumberGenerator randomNumberGenerator)
        {
            RandomNumberGenerator = randomNumberGenerator;
        }

        /// <summary>
        /// If non-null, defines channel(s) for which the handler will run.  Default = all channels (null)
        /// </summary>
        public virtual string[] TargetedChannels => AllChannels;

        /// <summary>
        /// If true, the handler will only run when hsbot is directly mentioned by the message.  Default = true
        /// </summary>
        public virtual bool DirectMentionOnly => true;

        /// <summary>
        /// Odds that a handler will run - should be between 0.0 and 1.0.
        /// If less than 1.0, a random roll will happen for each incoming message
        /// to the handler to determine if the handler will actually return any message.
        /// </summary>
        public virtual double GetHandlerOdds(InboundMessage message)
        {
            //there's a 110% chance this handler will run by default!
            //in other words, we avoid the whole floating-point-error
            //thing by returning a value much greater than 1 to ensure
            //the handler always runs in the default case
            return 1.1;
        }

        public string GetRandomCannedResponse(params object[] args)
        {
            if (CannedResponses == null || CannedResponses.Length == 0)
            {
                throw new Exception("CannedResponses list cannot be empty");
            }

            var randomCannedResponse = RandomNumberGenerator.GetRandomValue(CannedResponses);
            return string.Format(randomCannedResponse, args);
        }

        public abstract IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors();


        public HandlesResult Handles(InboundMessage message)
        {
            var handlerOdds = GetHandlerOdds(message);
            var canHandleMessage = CanHandle(message);
            var randomRoll = RandomNumberGenerator.Generate();

            var shouldHandle = (!DirectMentionOnly || message.BotIsMentioned)
                               && (TargetedChannels == AllChannels || message.IsForChannel(TargetedChannels))
                               && (handlerOdds >= 1.0 || randomRoll < handlerOdds)
                               && canHandleMessage;

            return new HandlesResult
            {
                HandlesMessage = shouldHandle,
                HandlerDirectionMentionOnly = DirectMentionOnly,
                BotIsMentioned = message.BotIsMentioned,
                HandlerTargetedChannels = TargetedChannels,
                MessageChannel = message.ChannelName,
                HandlerOdds = handlerOdds,
                RandomRoll = randomRoll,
                HandlerCanHandleResult = canHandleMessage
            };
        }

        protected abstract bool CanHandle(InboundMessage message);
        public abstract Task HandleAsync(IInboundMessageContext context);
    }
}
