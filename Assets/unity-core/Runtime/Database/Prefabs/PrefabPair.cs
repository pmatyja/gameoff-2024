using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PrefabPair : IEqualityComparer<PrefabPair>
{
    public static IEqualityComparer<PrefabPair> DefaultEqualityComparer = new PrefabPair();

    public PrefabSO AdjacentPrefab;
    public int AdjacentRotation;

    public PrefabSO CenterPrefab;
    public int CenterRotation;

    public Vector3Int AdjacentOffset;

    public PrefabPair()
    {
    }

    public PrefabPair(PrefabSO centerPrefab, int centerRotation, PrefabSO adjacentPrefab, int adjacentRotation, Vector3Int adjacentOffset)
    {
        this.AdjacentPrefab = adjacentPrefab;
        this.AdjacentRotation = adjacentRotation;
        this.CenterPrefab = centerPrefab;
        this.CenterRotation = centerRotation;
        this.AdjacentOffset = adjacentOffset;
    }

    public int CompareTo(PrefabPair other)
    {
        if (this.CenterPrefab == other.CenterPrefab &&
            this.CenterRotation == other.CenterRotation &&
            this.AdjacentPrefab == other.AdjacentPrefab &&
            this.AdjacentRotation == other.AdjacentRotation)
        {
            return 0;
        }

        return 1;
    }

    public bool Equals(PrefabPair left, PrefabPair right)
    {
        return 
            left.CenterPrefab       == right.CenterPrefab &&
            left.CenterRotation     == right.CenterRotation &&
            left.AdjacentPrefab     == right.AdjacentPrefab &&
            left.AdjacentRotation   == right.AdjacentRotation;
    }

    public int GetHashCode(PrefabPair obj)
    {
        var hash = HashCode.Combine(this.CenterRotation, this.AdjacentRotation);

        if (this.CenterPrefab)
        {
            hash = HashCode.Combine(hash, this.CenterPrefab.GetHashCode());
        }

        if (this.AdjacentPrefab)
        {
            hash = HashCode.Combine(hash, this.AdjacentPrefab.GetHashCode());
        }

        return hash;
    }
}