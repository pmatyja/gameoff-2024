using System;
using UnityEngine;

[Serializable]
public class NodeInfo
{
    [Readonly]
    public string Id = Guid.NewGuid().ToString();

    [Readonly]
    public Vector2 Position;

    [Readonly]
    public string Type;

    [SerializeReference]
    public INode Properties;

    public void OnEnable()
    {
        this.Properties?.OnEnable();
    }

    public void OnDisable()
    {
        this.Properties?.OnDisable();
    }
}
