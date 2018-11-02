namespace Hsbot.Core.Random
{
  public class RandomNumberGenerator : IRandomNumberGenerator
  {
    private readonly System.Random _rng;

    public RandomNumberGenerator()
    {
      _rng = new System.Random();
    }

    public RandomNumberGenerator(int seed)
    {
      _rng = new System.Random(seed);
    }

    
    /// <inheritdoc />
    public double Generate()
    {
      return _rng.NextDouble();
    }

    /// <inheritdoc />
    public int Generate(int minValue, int maxValue)
    {
        return _rng.Next(minValue, maxValue);
    }
  }
}