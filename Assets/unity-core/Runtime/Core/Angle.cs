using System.Runtime.CompilerServices;

public static class Angle
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToRadians(this float value)
    {
        return value * 0.01745329252f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToDegrees(this float value)
    {
        return value * 57.295779513f;
    }
}
