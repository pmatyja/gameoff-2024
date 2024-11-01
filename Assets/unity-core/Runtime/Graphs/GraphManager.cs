using System;
using System.Collections.Concurrent;
using UnityEngine;

public class GraphManager : Singleton<GraphManager>
{
    private static readonly ConcurrentDictionary<Guid, INodeGraphContext> Workers = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RunOnStart()
    {
        Workers.Clear();
    }

    private void OnDisable()
    {
        Workers.Clear();
        this.StopAllCoroutines();
    }
}
