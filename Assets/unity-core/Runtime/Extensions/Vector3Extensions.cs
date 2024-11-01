using System.Runtime.CompilerServices;
using UnityEngine;

public static class Vector3Extensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 SnapToGrid(this Vector3 position)
    {
        return new Vector3
        (
            Mathf.RoundToInt(position.x),
            Mathf.RoundToInt(position.y),
            Mathf.RoundToInt(position.z)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 SnapToFloor(this Vector3 position)
    {
        return new Vector3
        (
            Mathf.RoundToInt(position.x),
            Mathf.RoundToInt(Mathf.Max(position.y, 0.0f)),
            Mathf.RoundToInt(position.z)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetConnectionMask(this Vector3Int direction)
    {
        return ((direction.x + 1) + 6) | ((direction.y + 1) + 3) | ((direction.z + 1));
    }
}