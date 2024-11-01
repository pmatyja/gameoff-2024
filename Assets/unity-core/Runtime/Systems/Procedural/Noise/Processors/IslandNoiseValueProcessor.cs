using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class IslandNoiseValueProcessor : NoiseValueProcessor
{
    [Range(0.0f, 0.5f)]
    public float LinearShaping = 0.5f;

    [Range(0.0f, 0.5f)]
    public float DistanceOffset = 0.2f;

    [Range(1, short.MaxValue)]
    public int Width = 128;

    [Range(1, short.MaxValue)]
    public int Height = 128;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Process(float value, float x, float y = 1.0f)
    {
        var nx = 2.0f * x / this.Width  - 1.0f;
        var ny = 2.0f * y / this.Height - 1.0f;

        var distance = Mathf.Clamp01( 1.0f - ( 1.0f - ( nx * nx ) ) * ( 1.0f - ( ny * ny ) ) );

        return Mathf.Lerp( value, 1.0f - distance, this.LinearShaping ) - this.DistanceOffset;
    }
}