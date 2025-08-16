using System;

namespace KB.SharpCore.Utils;

/// <summary>
/// Extension methods for <see cref="Random"/> to provide additional functionality.
/// </summary>
public static class RandomExtension
{
    public static float NextSingle(this Random rng, float minValue, float maxValue)
    {
        if (minValue >= maxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(minValue), "minValue must be less than maxValue.");
        }

        return (float)(rng.NextDouble() * (maxValue - minValue) + minValue);
    }

    public static float NextSingle(this Random rng, float maxValue)
    {
        if (maxValue <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxValue), "maxValue must be greater than 0.");
        }

        return (float)(rng.NextDouble() * maxValue);
    }
}
