using Hsbot.Slack.Core.Random;

namespace Hsbot.Slack.Core.Tests.MessageHandler
{
    public class RandomNumberGeneratorFake : IRandomNumberGenerator
    {
        public double NextDoubleValue { get; set; }

        public double Generate()
        {
            return NextDoubleValue;
        }
    }
}