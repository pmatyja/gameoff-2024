using System.Runtime.CompilerServices;
using UnityEngine;

public static class RectIntExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RectInt FromCoordinates(int x, int y, int width, int height, int unitSize = 1)
    {
        return new RectInt(x * width * unitSize, y * height * unitSize, width * unitSize, height * unitSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RectInt Inflate(this RectInt rectangle, int value)
    {
        return new RectInt(rectangle.x - value, rectangle.y - value, rectangle.width + value * 2, rectangle.height + value * 2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(this RectInt rect, float x, float y)
    {
        return rect.xMin <= x && x < rect.xMax && rect.xMin <= y && y < rect.yMax;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(this RectInt other, RectInt rectangle)
    {
        return other.xMin <= rectangle.xMin && other.yMin <= rectangle.yMin && other.xMax >= rectangle.xMax && other.yMax >= rectangle.yMax;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(RectInt a, RectInt b)
    {
        return (b.xMin < a.xMax) && (a.xMin < b.xMax) && (b.yMin < a.yMax) && (a.yMin < b.yMax);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RectInt Union(RectInt a, RectInt b)
    {
        var x1 = Mathf.Min(a.x, b.x);
        var x2 = Mathf.Max(a.x + a.width, b.x + b.width);
        var y1 = Mathf.Min(a.y, b.y);
        var y2 = Mathf.Max(a.y + a.height, b.y + b.height);

        return new RectInt(x1, y1, x2 - x1, y2 - y1);
    }
}