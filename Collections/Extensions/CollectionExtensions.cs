using System;
namespace KB.SharpCore.Collections.Extensions;
public static class CollectionExtensions
{
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        Random rng = new Random();
        return source.OrderBy(x => rng.Next());
    }
}
