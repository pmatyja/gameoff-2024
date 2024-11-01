using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class RedistributionNoiseValueProcessor : NoiseValueProcessor
{
    [Range(0.0f, 4.0f)]
    public float Redistrubution = 1.0f;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Process(float value, float x, float y = 1.0f)
    {
        return Mathf.Pow(value, this.Redistrubution);
    }
}