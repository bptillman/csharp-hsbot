using System.Collections.Generic;
using Hsbot.Slack.Core.Messaging;
using Hsbot.Slack.Core.Random;

namespace Hsbot.Slack.Core.MessageHandlers
{
    public abstract class MessageHandlerBase : IInboundMessageHandler
    {
      protected readonly IRandomNumberGenerator RandomNumberGenerator;

      public static readonly string[] AllChannels = null;
      public static readonly string[] FunChannels = {"#general", "#developers", "#austin", "#houston", "#dallas", "#monterrey", "#hsbottesting"};

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

      public abstract IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors();

      public bool Handles(InboundMessage message)
      {
        var handlerOdds = GetHandlerOdds(message);

        return (!DirectMentionOnly || message.BotIsMentioned)
               && (TargetedChannels == AllChannels || message.IsForChannel(TargetedChannels))
               && (handlerOdds >= 1.0 || RandomNumberGenerator.Generate() < handlerOdds)
               && CanHandle(message);
      }

      protected abstract bool CanHandle(InboundMessage message);

      public abstract IEnumerable<OutboundResponse> Handle(InboundMessage message);
    }
}
