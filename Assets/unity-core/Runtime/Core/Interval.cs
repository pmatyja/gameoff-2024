using System.Runtime.CompilerServices;
using System;
using UnityEngine;

public static class Interval
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Check(ref float time, float interval, float rateOfChange)
    {
        time += rateOfChange;

        if (time >= interval)
        {
            time = 0.0f;
            return true;
        }

        return false;
    }
}