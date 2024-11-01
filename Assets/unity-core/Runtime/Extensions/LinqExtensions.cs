using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public static class LinqExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Any<T>(this IReadOnlyCollection<T> items)
    {
        return items.Count > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Any<T>(this IReadOnlyList<T> items, Func<T, bool> predicate)
    {
        for (var i = 0; i < items.Count; ++i)
        {
            if (predicate.Invoke(items[i]))
            {
                return true;
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool All<T>(this IReadOnlyList<T> items, Func<T, bool> predicate)
    {
        for (var i = 0; i < items.Count; ++i)
        {
            if (predicate.Invoke(items[i]) == false)
            {
                return false;
            }
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains<T>(this IReadOnlyList<T> items, T item) where T : IEquatable<T>
    {
        for (var i = 0; i < items.Count; ++i)
        {
            if (items[i].Equals(item))
            {
                return true;
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains<T>(this IReadOnlyList<T> items, T item, IEqualityComparer<T> comparer = null)
    {
        comparer ??= EqualityComparer<T>.Default;
        
        for (var i = 0; i < items.Count; ++i)
        {
            if (comparer.Equals(items[i], item))
            {
                return true;
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T First<T>(this IReadOnlyList<T> items)
    {
        return items[0];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T FirstOrDefault<T>(this IReadOnlyList<T> items)
    {
        if (items.Count > 0)
        {
            return items[0];
        }

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T FirstOrDefault<T>(this IReadOnlyList<T> items, Func<T, bool> predicate)
    {
        for (var i = 0; i < items.Count; ++i)
        {
            if (predicate.Invoke(items[i]))
            {
                return items[i];
            }
        }

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T FirstOrDefault<T>(this IReadOnlyList<T> items, int skip)
    {
        if (items.Count > skip)
        {
            return items[skip];
        }

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T FirstOrDefault<T>(this IReadOnlyList<T> items, int skip, Func<T, bool> predicate)
    {
        for (var i = skip; i < items.Count; ++i)
        {
            if (predicate.Invoke(items[i]))
            {
                return items[i];
            }
        }

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ForEach<T>(this IReadOnlyList<T> items, Func<T, T> iterate)
    {
        for (var i = 0; i < items.Count; ++i)
        {
            iterate.Invoke(items[i]);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ForEachReturn<T>(this IReadOnlyList<T> items, Func<T, T, T> iterate)
    {
        var result = default(T);

        for (var i = 0; i < items.Count; ++i)
        {
            result = iterate.Invoke(result, items[i]);
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ForEachReturn<T>(this IReadOnlyList<T> items, T start, Func<T, T, T> iterate)
    {
        var result = start;

        for (var i = 0; i < items.Count; ++i)
        {
            result = iterate.Invoke(result, items[i]);
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ForEachReturn<T>(this IReadOnlyList<T> items, T start, Func<T, T, T> iterate, Func<T, T> last)
    {
        var result = start;

        for (var i = 0; i < items.Count; ++i)
        {
            result = iterate.Invoke(result, items[i]);
        }

        return last.Invoke(result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V ForEachReturn<K, V>(this IReadOnlyDictionary<K, V> items, Func<V, V, V> iterate)
    {
        var result = default(V);

        foreach (var pair in items)
        {
            result = iterate.Invoke(result, pair.Value);
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V ForEachReturn<K, V>(this IReadOnlyDictionary<K, V> items, V start, Func<V, V, V> iterate)
    {
        var result = start;

        foreach (var pair in items)
        {
            result = iterate.Invoke(result, pair.Value);
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Last<T>(this IReadOnlyList<T> items)
    {
        return items[items.Count - 1];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T LastOrDefault<T>(this IReadOnlyList<T> items)
    {
        if (items.Count > 0)
        {
            return items[items.Count - 1];
        }

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T LastOrDefault<T>(this IReadOnlyList<T> items, Func<T, bool> predicate)
    {
        for (var i = items.Count - 1; i >= 0 ; i--)
        {
            if (predicate.Invoke(items[i]))
            {
                return items[i];
            }
        }

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T FindBest<T>(this IList<T> items, Func<T, T, bool> isBetterMatch)
    {
        var result = default(T);

        for (var i = 0; i < items.Count; ++i)
        {
            if (isBetterMatch.Invoke(result, items[i]))
            {
                result = items[i];
            }
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T FindClosest<T>(this IList<T> items, Func<T, float> getScore)
    {
        var bestScore = float.MaxValue;
        var result = default(T);

        for (var i = 0; i < items.Count; ++i)
        {
            var current = getScore.Invoke(items[i]);
            if (current < bestScore)
            {
                bestScore = current;
                result = items[i];
            }
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Update<T>(this IList<T> items, Func<T, T> iterate)
    {
        for (var i = 0; i < items.Count; ++i)
        {
            items[i] = iterate.Invoke(items[i]);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T SkipOrDefault<T>(this IList<T> items, int index)
    {
        if (index < 0 || index >= items.Count)
        {
            return default;
        }

        return items[index];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items)
    {
        return items.Shuffle(Rng.RandomSeed());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items, ulong seed)
    {
        return System.Linq.Enumerable.OrderBy<T, int>(items, x => Rng.Range(ref seed, System.Linq.Enumerable.Count<T>(items) - 1));
    }
}