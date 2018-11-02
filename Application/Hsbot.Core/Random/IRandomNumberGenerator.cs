namespace Hsbot.Core.Random
{
    public interface IRandomNumberGenerator
    {
      /// <summary>
      /// Generates a random number between 0.0 and 1.0
      /// </summary>
      /// <returns>Random number between 0.0 and 1.0</returns>
      double Generate();
      
      /// <summary>
      /// Generates a random number within the given range
      /// </summary>
      /// <returns>Random number between the given range</returns>
      int Generate(int minValue, int maxValue);
    }
}
