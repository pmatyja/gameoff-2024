using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class MultiplerNoiseValueProcessor : NoiseValueProcessor
{
    [Range(0.0f, 10.0f)]
    public float Multipler = 1.0f;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Process(float value, float x, float y = 1.0f)
    {
        value = this.Multipler * value;
        return value - Mathf.Floor(value);
    }
}