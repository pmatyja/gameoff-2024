using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class FloatExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ShorterAngleTo(this float current, float target)
    {
        return (target - current + 540) % 360 - 180;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float RoundUp(this float value)
    {
        return (float)Math.Round(value, 0, MidpointRounding.AwayFromZero);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float RoundUp(this float value, float min)
    {
        return Mathf.Max(min, (float)Math.Round(value, 0, MidpointRounding.AwayFromZero));
    }
}