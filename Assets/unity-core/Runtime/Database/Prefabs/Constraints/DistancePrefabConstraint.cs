using System;
using UnityEngine;

[Serializable]
public struct SeparationPrefabConstraint : IPrefabConstraint
{
    public bool Enabled;

    [Range(0, 10)]
    public int MinDistance;
}
