using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class RidgeNoiseValueProcessor : NoiseValueProcessor
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Process(float value, float x, float y = 1.0f)
    {
        return 2.0f * ( 0.5f - Mathf.Abs( 0.5f - value ) );
    }
}