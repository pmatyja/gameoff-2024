using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class PersistenceOctaveGenerator : OctaveGenerator
{
    [Range(1, 8)]
    public int Octaves = 6;

    [Range(0.0f, 4.0f)]
    public float Persistence = 0.5f;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Generate(float lacunarity, float x, float y = 1.0f)
    {
        var frequnecy = 1.0f;
        var amplitude = 1.0f;

        var total = 0.0f;
        var value = 0.0f;

        for (var i = 0; i < this.Octaves; i++)
        {
            total += amplitude;
            value += amplitude * Mathf.PerlinNoise(x * frequnecy, y * frequnecy);

            frequnecy *= lacunarity;
            amplitude *= this.Persistence;
        }

        return value / total;
    }
}
