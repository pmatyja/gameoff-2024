using System;
using System.Runtime.CompilerServices;

public static class Rng
{
    public static ulong Seed = 1;
    public static ulong Multiplier = 0x2545F4914F6CDD1Dul;

    private const float FloatConversion = 1.0f / (int.MaxValue + 1.0f);
    private const string Characters = "abcdefghijklmnoprstquvxyz0123456789";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong RandomSeed()
    {
        return (ulong)(Guid.NewGuid().GetHashCode() | Guid.NewGuid().GetHashCode() << 32);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong OffsetSeed(ulong seed, int x, int y, long offset = int.MaxValue)
    {
        return seed + (ulong)(offset + x) | (ulong)(offset + y) << 32;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong NextSeed(ref ulong seed)
    {
        seed ^= seed >> 12;
        seed ^= seed << 25;
        seed ^= seed >> 27;

        return seed;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Int()
    {
        return Int(ref Seed);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Int(ref ulong seed)
    {
        var nextSeed = NextSeed(ref seed);
        return 0x7FFFFFFF & (int)((nextSeed * Multiplier) >> 32);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Float(ref ulong seed)
    {
        return Int(ref seed) * FloatConversion;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float FloatUniform(ref ulong seed, int baseNumber = 4096)
    {
        return 1.0f - (float)(Int(ref seed) % baseNumber) / (baseNumber / 2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float FloatTriangle(ref ulong seed)
    {
        return (Float(ref seed) + Float(ref seed)) / 2.0f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float FloatGaussian(ref ulong seed)
    {
        return Float(ref seed) * Float(ref seed);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Boolean(ref ulong seed)
    {
        return Float(ref seed) >= 0.5f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sign(ref ulong seed)
    {
        return ((Int(ref seed) & 0x1) == 0) ? -1 : 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Radian(ref ulong seed)
    {
        return Float(ref seed) * 6.28318530718f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Degree(ref ulong seed)
    {
        return Float(ref seed) * 360.0f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Range(ref ulong seed, int max)
    {
        return Int(ref seed) % max;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Range(ref ulong seed, int min, int max)
    {
        return Int(ref seed) % (max - min) + min;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Range(ref ulong seed, float max)
    {
        return Float(ref seed) * max;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Range(ref ulong seed, float min, float max)
    {
        return Float(ref seed) * (max - min) + min;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Id(ref ulong seed, int length = 5)
    {
        var result = string.Empty;

        for (var i = 0; i < length; ++i)
        {
            result += Characters[Range(ref seed, Characters.Length - 1) % length];
        }

        return result;
    }
}
