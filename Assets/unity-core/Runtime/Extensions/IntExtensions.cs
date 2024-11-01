using System.Runtime.CompilerServices;
using UnityEngine;

public static class IntExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color ToColorHue(this int value, int numberOfColors = 64)
    {
        return Color.HSVToRGB((float)value / (float) numberOfColors, 0.8f, 0.6f);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToLocalIndex(this int value, int blockSize)
    {
        return (value % blockSize + blockSize) % blockSize;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToGlobalIndex(this int value, int blockSize)
    {
        if (value < 0)
        {
            return ( value + 1 ) / blockSize - 1;
        }

        return value / blockSize;
    }
}