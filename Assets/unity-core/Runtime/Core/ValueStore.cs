using System.Collections.Concurrent;
using UnityEngine;

public static class ValueStore
{
    private static readonly ConcurrentDictionary<object, object> Values = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void RunOnStart()
    {
        Values.Clear();
    }

    public static bool TryGet<T>(object key, out T result, T defaultValue = default)
    {
        if (Values.TryGetValue(key, out var value))
        {
            if (value is T validResult)
            {
                result = validResult;
                return true;
            }
        }

        result = defaultValue;
        return false;
    }

    public static T Get<T>(object key, T defaultValue = default)
    {
        if (Values.TryGetValue(key, out var value))
        {
            if (value is T validResult)
            {
                return validResult;
            }
        }

        return defaultValue;
    }

    public static void Set<T>(object key, T value)
    {
        Values[key] = value;
    }
}
