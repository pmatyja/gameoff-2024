using System;
using DeBroglie.Constraints;
using UnityEngine;

[Serializable]
public struct CountPrefabConstraint : IPrefabConstraint
{
    public bool Enabled;

    [Range(0, 5)]
    public int Count;

    public CountComparison Comparison;
}
