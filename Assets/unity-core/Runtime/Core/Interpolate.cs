using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class Interpolation
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Interpolate(float value1, float value2, float amount, Func<float, float> interpolate)
    {
        return value1 + (value2 - value1) * interpolate.Invoke(amount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Interpolate(Vector2 value1, Vector2 value2, float amount, Func<float, float, float, float> interpolate)
    {
        return new Vector2
		(
			interpolate.Invoke(value1.x, value2.x, amount),
			interpolate.Invoke(value1.y, value2.y, amount)
		);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Interpolate(Vector2 value1, Vector2 value2, Vector2 value3, float amount, Func<float, float, float, float, float> interpolate)
    {
        return new Vector2
		(
			interpolate.Invoke(value1.x, value2.x, value3.x, amount),
			interpolate.Invoke(value1.y, value2.y, value3.y, amount)
		);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 Interpolate(Vector2 value1, Vector2 value2, Vector2 value3, Vector2 value4, float amount, Func<float, float, float, float, float, float> interpolate)
    {
        return new Vector2
		(
			interpolate.Invoke(value1.x, value2.x, value3.x, value4.x, amount),
			interpolate.Invoke(value1.y, value2.y, value3.y, value4.y, amount)
		);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 Interpolate(Vector3 value1, Vector3 value2, float amount, Func<float, float, float, float> interpolate)
    {
        return new Vector3
		(
			interpolate.Invoke(value1.x, value2.x, amount),
			interpolate.Invoke(value1.y, value2.y, amount),
			interpolate.Invoke(value1.z, value2.z, amount)
		);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 Interpolate(Vector3 value1, Vector3 value2, Vector3 value3, float amount, Func<float, float, float, float, float> interpolate)
    {
        return new Vector3
		(
			interpolate.Invoke(value1.x, value2.x, value3.x, amount),
			interpolate.Invoke(value1.y, value2.y, value3.y, amount),
			interpolate.Invoke(value1.z, value2.z, value3.z, amount)
		);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Interpolate(Vector3 value1, Vector3 value2, Vector3 value3, Vector3 value4, float amount, Func<float, float, float, float, float, float> interpolate)
    {
        return new Vector3
		(
			interpolate.Invoke(value1.x, value2.x, value3.x, value4.x, amount),
			interpolate.Invoke(value1.y, value2.y, value3.y, value4.y, amount),
			interpolate.Invoke(value1.z, value2.z, value3.z, value4.z, amount)
		);
    }
}
