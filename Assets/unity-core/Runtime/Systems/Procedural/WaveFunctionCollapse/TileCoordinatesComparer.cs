using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TileCoordinatesComparer : IComparer<Vector3Int>, IEqualityComparer<Vector3Int>
{
    public static readonly TileCoordinatesComparer Default = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compare(Vector3Int left, Vector3Int right)
    {
        if (left.z > right.z)
        {
            return -1;
        }

        if (left.z < right.z)
        {
            return 1;
        }

        if (left.x < right.x)
        {
            return -1;
        }

        if (left.x > right.x)
        {
            return 1;
        }

        return 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Vector3Int first, Vector3Int second)
    {
        return first.x == second.x && first.z == second.z;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetHashCode(Vector3Int obj)
    {
        return HashCode.Combine(obj.x.GetHashCode(), obj.z.GetHashCode());
    }
}