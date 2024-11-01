using System.Runtime.CompilerServices;
using UnityEngine;

public static class LayerMaskExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsLayer(this LayerMask mask, int layer)
    {
        return (mask & (1 << layer)) > 0; 
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(this int mask, int layer)
    {
        return (mask & (1 << layer)) > 0;
    }
}
