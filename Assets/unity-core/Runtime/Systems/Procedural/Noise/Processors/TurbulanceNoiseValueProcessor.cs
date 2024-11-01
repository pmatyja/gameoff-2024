using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class TurbulanceNoiseValueProcessor : NoiseValueProcessor
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Process(float value, float x, float y = 1.0f)
    {
        return Mathf.Abs(value - 0.5f * 2.0f);
    }
}