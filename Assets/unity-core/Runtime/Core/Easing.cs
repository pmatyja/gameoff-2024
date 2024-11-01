using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class Easing
{
    private const float HalfPi = Mathf.PI / 2.0f;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float BackIn(float value)
    {
        return value * value * value - value * MathF.Sin(value * MathF.PI);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float BackInOut(float value)
    {
        if (value < 0.5f)
        {
            var f = 2.0f * value;
            return 0.5f * (f * f * f - f * MathF.Sin(f * MathF.PI));
        }
        else
        {
            var f = (1.0f - (2.0f * value - 1.0f));
            return 0.5f * (1.0f - (f * f * f - f * MathF.Sin(f * MathF.PI))) + 0.5f;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float BackOut(float value)
    {
        var f = (1.0f - value);
        return 1.0f - (f * f * f - f * MathF.Sin(f * MathF.PI));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float BounceInOut(float value)
    {
        if (value < 0.5f)
        {
            return 0.5f * BounceIn(value * 2.0f);
        }

        return 0.5f * BounceOut(value * 2.0f - 1.0f) + 0.5f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float BounceIn(float value)
    {
        return 1.0f - BounceOut(1.0f - value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float BounceOut(float value)
    {
        if (value < 4.0f / 11.0f)
        {
            return (121.0f * value * value) / 16.0f;
        }

        if (value < 8.0f / 11.0f)
        {
            return (363.0f / 40.0f * value * value) - (99.0f / 10.0f * value) + 17.0f / 5.0f;
        }
        
        if (value < 9.0f / 10.0f)
        {
            return (4356.0f / 361.0f * value * value) - (35442.0f / 1805.0f * value) + 16061.0f / 1805.0f;
        }
        
        return (54.0f / 5.0f * value * value) - (513.0f / 25.0f * value) + 268.0f / 25.0f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float CircularInOut(float value)
    {
        if (value < 0.5f)
        {
            return 0.5f * (1.0f - MathF.Sqrt(1.0f - 4.0f * (value * value)));
        }

        return 0.5f * (MathF.Sqrt(-((2.0f * value) - 3.0f) * ((2.0f * value) - 1.0f)) + 1.0f);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float CircularIn(float value)
    {
        return 1.0f - MathF.Sqrt(1.0f - (value * value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float CircularOut(float value)
    {
        return MathF.Sqrt((2.0f - value) * value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float CubicInOut(float value)
    {
        if (value < 0.5f)
        {
            return 4.0f * value * value * value;
        }

        var f = ((2.0f * value) - 2.0f);
        return 0.5f * f * f * f + 1.0f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float CubicIn(float value)
    {
        return value * value * value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float CubicOut(float value)
    {
        var f = (value - 1.0f);
        return f * f * f + 1.0f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ElasticIn(float value)
    {
        return MathF.Sin(13.0f * HalfPi * value) * MathF.Pow(2.0f, 10.0f * (value - 1.0f));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ElasticInOut(float value)
    {
        if (value < 0.5f)
        {
            return 0.5f * MathF.Sin(13.0f * HalfPi * (2.0f * value)) * MathF.Pow(2.0f, 10.0f * ((2.0f * value) - 1.0f));
        }

        return 0.5f * (MathF.Sin(-13.0f * HalfPi * ((2.0f * value - 1.0f) + 1.0f)) * MathF.Pow(2.0f, -10.0f * (2.0f * value - 1.0f)) + 2.0f);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ElasticOut(float value)
    {
        return MathF.Sin(-13.0f * HalfPi * (value + 1.0f)) * MathF.Pow(2.0f, -10.0f * value) + 1.0f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ExponentialInOut(float value)
    {
        if (MathF.Abs(value) < float.Epsilon || MathF.Abs(value - 1.0f) < float.Epsilon)
        {
            return value;
        }

        if (value < 0.5f)
        {
            return 0.5f * MathF.Pow(2.0f, (20.0f * value) - 10.0f);
        }

        return -0.5f * MathF.Pow(2.0f, (-20.0f * value) + 10.0f) + 1.0f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ExponentialIn(float value)
    {
        return (MathF.Abs(value) < float.Epsilon) ? value : MathF.Pow(2.0f, 10.0f * (value - 1.0f));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ExponentialOut(float value)
    {
        return (MathF.Abs(value - 1.0f) < float.Epsilon) ? value : 1.0f - MathF.Pow(2.0f, -10.0f * value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Linear(float value)
    {
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float QuadraticInOut(float value)
    {
        if (value < 0.5f)
        {
            return 2.0f * value * value;
        }

        return (-2.0f * value * value) + (4.0f * value) - 1.0f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float QuadraticIn(float value)
    {
        return value * value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float QuadraticOut(float value)
    {
        return -(value * (value - 2.0f));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float QuarticInOut(float value)
    {
        if (value < 0.5f)
        {
            return 8.0f * value * value * value * value;
        }

        var f = (value - 1.0f);
        return -8.0f * f * f * f * f + 1.0f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float QuarticIn(float value)
    {
        return value * value * value * value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float QuarticOut(float value)
    {
        var f = (value - 1.0f);
        return f * f * f * (1.0f - value) + 1.0f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float QuinticInOut(float value)
    {
        if (value < 0.5f)
        {
            return 16.0f * value * value * value * value * value;
        }

        var f = ((2.0f * value) - 2.0f);
        return 0.5f * f * f * f * f * f + 1.0f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float QuinticIn(float value)
    {
        return value * value * value * value * value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float QuinticOut(float value)
    {
        var f = (value - 1.0f);
        return f * f * f * f * f + 1.0f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float SineInOut(float value)
    {
        return 0.5f * (1.0f - MathF.Cos(value * MathF.PI));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float SineIn(float value)
    {
        return MathF.Sin((value - 1.0f) * HalfPi) + 1.0f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float SineOut(float value)
    {
        return MathF.Sin(value * HalfPi);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float SmoothStep(float amount)
    {
        return amount * amount * (3f - 2f * amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float SmoothStep(float amount, float start, float end)
    {
        var k = Math.Clamp((amount - start) / (end - start), 0.0f, 1.0f);
        return k * k * (3f - 2f * k);
    }
}
