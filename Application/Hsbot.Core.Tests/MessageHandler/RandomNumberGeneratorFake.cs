using Hsbot.Core.Random;

namespace Hsbot.Core.Tests.MessageHandler
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