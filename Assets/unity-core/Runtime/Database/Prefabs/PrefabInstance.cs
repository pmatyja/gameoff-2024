using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class PrefabInstance
{
    public Transform Parent { get; private set; }
    public GameObject Instance { get; private set; }
    public PrefabSO Prefab;

    public PrefabInstance(Transform parent)
    {
        this.Parent = parent;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public GameObject Create()
    {
        this.Destroy();
        this.Instance = this.Parent.CreateChild(this.Prefab);
        return this.Instance;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Destroy()
    {
        if (this.Instance != null)
        {
            GameObject.Destroy(this.Instance);
        }
    }
}