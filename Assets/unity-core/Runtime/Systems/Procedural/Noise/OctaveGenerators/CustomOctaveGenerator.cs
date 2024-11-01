using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class CustomOctaveGenerator : OctaveGenerator
{
    public float[] Amplitudes = new[]
    {
        1.0f,
        0.5f,
        0.13f,
        0.06f,
        0.03f
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Generate(float lacunarity, float x, float y = 1.0f)
    {
        if (this.Amplitudes == null || this.Amplitudes.Length < 1)
        {
            return Mathf.PerlinNoise(x, y);
        }

        var frequnecy = 1.0f;

        var total = 0.0f;
        var value = 0.0f;

        for (var i = 1; i < this.Amplitudes.Length; i++)
        {
            total += this.Amplitudes[i];
            value += this.Amplitudes[i] * Mathf.PerlinNoise(x * frequnecy, y * frequnecy);

            frequnecy *= lacunarity;
        }

        return value / total;
    }
}
