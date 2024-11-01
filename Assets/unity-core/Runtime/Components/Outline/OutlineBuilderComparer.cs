using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class OutlineBuilderComparer : IComparer<Vector3>, IEqualityComparer<Vector3>
{
    public static readonly OutlineBuilderComparer Default = new();

    public int Compare(Vector3 left, Vector3 right)
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
    public bool Equals(Vector3 first, Vector3 second)
    {
        return first.Equals(second);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetHashCode(Vector3 obj)
    {
        return obj.GetHashCode();
    }
}
