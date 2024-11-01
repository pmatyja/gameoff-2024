using System.Collections.Concurrent;
using UnityEngine;

public abstract class AudioSources
{
    public static ConcurrentDictionary<string, IAudioSource> Sources = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RunOnStart()
    {
        Sources.Clear();
    }

    public static IAudioSource Get(string id)
    {
        if (AudioSources.Sources.TryGetValue(id, out var result))
        {
            return result;
        }

        return default;
    }

    public static void Stop(AudioLayers layers)
    {
        foreach (var pair in AudioSources.Sources)
        {
            if (LayerMaskExtensions.ContainsLayer((int)layers, (int)pair.Value.Resource.Layer))
            {
                pair.Value.Stop();
            }
        }
    }
}
