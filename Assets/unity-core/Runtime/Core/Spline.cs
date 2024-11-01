using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class Spline
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Interpolate(float value1, float value2, float value3, float amount)
    {
        return (1.0f - amount) *
               (1.0f - amount) * value1 +
        2.0f * (1.0f - amount) * amount * value2 +
                       amount  * amount * value3;
    }

    public static void Open(IReadOnlyList<Vector3> points, Action<Vector3> onPoint, int pointsPerCurveSegment = 4)
    {
        if (points.Count < 3 || pointsPerCurveSegment < 2)
        {
            foreach (var point in points)
            {
                onPoint.Invoke(point);
            }
            return;
        }

        for (var index = -1; index < points.Count - 1; index++)
        {
            var i0 = Math.Max(index, 0);
            var i1 = Math.Max(index + 1, 0);
            var i2 = Math.Min(index + 2, points.Count - 1);

            var segmentDelta = 1.0f / pointsPerCurveSegment;

            if (index == points.Count - 2)
            {
                segmentDelta = 1.0f / (pointsPerCurveSegment - 1.0f);
            }

            var point0 = 0.5f * (points[i0] + points[i1]);
            var point2 = 0.5f * (points[i1] + points[i2]);

            for (var t = 0; t < pointsPerCurveSegment; t++)
            {
                onPoint.Invoke(Interpolation.Interpolate(point0, points[i1], point2, segmentDelta * t, Spline.Interpolate));
            }
        }
    }

    public static void Open(IReadOnlyList<Vector3> points, ICollection<Vector3> results, int pointsPerCurveSegment = 4)
    {
        results.Clear();
        Open(points, point => results.Add(point), pointsPerCurveSegment);
    }

    public static void Closed(IReadOnlyList<Vector3> points, Action<Vector3> onPoint, int pointsPerCurveSegment = 4)
    {
        if (points.Count < 3 || pointsPerCurveSegment < 2)
        {
            foreach (var point in points)
            {
                onPoint.Invoke(point);
            }
            return;
        }

        var segmentDelta = 1.0f / pointsPerCurveSegment;

        for (var i = -1; i < points.Count - 1; i++)
        {
            var i1 = (i + 1) % points.Count;
            var i2 = (i + 2) % points.Count;
            var i0 = i1 >= 1 ? i1 - 1 : points.Count - 1;

            var point0 = 0.5f * (points[i0] + points[i1]);
            var point2 = 0.5f * (points[i1] + points[i2]);

            for (var t = 0; t < pointsPerCurveSegment; t++)
            {
                onPoint.Invoke(Interpolation.Interpolate(point0, points[i1], point2, segmentDelta * t, Spline.Interpolate));
            }
        }
    }

    public static void Closed(IReadOnlyList<Vector3> points, ICollection<Vector3> results, int pointsPerCurveSegment = 4)
    {
        results.Clear();
        Closed(points, point => results.Add(point), pointsPerCurveSegment);
    }
}
