using System;
using Hsbot.Core.Random;

namespace Hsbot.Core.Tests.MessageHandler.Infrastructure
{
    public class RandomNumberGeneratorFake : IRandomNumberGenerator
    {
        private int _randomIntIndex = 0;
        private int[] _randomInts = {0};

        /// <summary>
        /// When set, calls to int Generate(int,int) cycle through this array for return values
        /// </summary>
        public int[] RandomIntValues
        {
            get => _randomInts;
            set
            {
                if (value == null || value.Length == 0)
                    throw new ArgumentException();

                _randomIntIndex = 0;
                _randomInts = value;
            }
        }

        /// <summary>
        /// When set, all calls to int Generate(int,int) return this value
        /// </summary>
        public int NextIntValue
        {
            get => RandomIntValues[_randomIntIndex];
            set => RandomIntValues = new[] { value };
        }

        private int _randomDoubleIndex = 0;
        private double[] _randomDoubles = {0.0};

        /// <summary>
        /// When set, all calls to double Generate()
        /// </summary>
        public double[] RandomDoubleValues
        {
            get => _randomDoubles;
            set
            {
                if (value == null || value.Length == 0)
                    throw new ArgumentException();

                _randomDoubleIndex = 0;
                _randomDoubles = value;
            }
        }

        /// <summary>
        /// When set, calls to double Generate() cycle through this array for return values
        /// </summary>
        public double NextDoubleValue
        {
            get => RandomDoubleValues[_randomDoubleIndex];
            set => RandomDoubleValues = new []{ value };
        }

        public double Generate()
        {
            var result = RandomDoubleValues[_randomDoubleIndex];
            _randomDoubleIndex = (_randomDoubleIndex + 1) % RandomDoubleValues.Length;
            return result;
        }

        public int Generate(int minValue, int maxValue)
        {
            var result = RandomIntValues[_randomIntIndex];
            _randomIntIndex = (_randomIntIndex + 1) % RandomIntValues.Length;
            return result;
        }
    }
}