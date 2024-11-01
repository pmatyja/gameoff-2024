using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class Collision
{
    [ThreadStatic]
    private static Collider[] CollidersBuffer = new Collider[32];

    [ThreadStatic]
    private static Collider2D[] CollidersBuffer2D = new Collider2D[32];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(float cx, float cy, float radius, float rx, float ry, float width, float height)
    {
        var testX = cx;
        var testY = cy;
        
        if (cx < rx)                testX = rx;
        else if (cx > rx + width)   testX = rx + width;

        if (cy < ry)                testY = ry;
        else if (cy > ry + height)  testY = ry + height;
        
        var distX = cx - testX;
        var distY = cy - testY;
        
        return Mathf.Sqrt( (distX * distX) + (distY * distY) ) <= radius;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(float cx, float cy, float radius, float rx, float ry, float width, float height, out IntersectionResult result)
    {
        var closest = new Vector2
        (
            Mathf.Clamp(cx, rx, rx + width),
            Mathf.Clamp(cy, ry, ry + height)
        );

        var direction = new Vector2(cx - closest.x, cy - closest.y);
        var distance = direction.sqrMagnitude;

        if (distance > 0 && distance < radius)
        {
            result = new IntersectionResult
            {
                Result = true,
                Distance = radius - distance,
                Normal = direction.normalized,
                Point = closest
            };
        }
        else
        {
            result = new IntersectionResult
            {
                Result = false
            };
        }

        return result.Result;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(float x, float y, float radius, Rect rect)
    {
        return Collision.Intersects(x, y, radius, rect.x, rect.y, rect.width, rect.height);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(float x, float y, float radius, RectInt rect)
    {
        return Collision.Intersects(x, y, radius, rect.x, rect.y, rect.width, rect.height);
    }
    
    public static void FieldOfView(Vector3 origin, Vector3 direction, Vector3 up, float angle, float range, LayerMask detectionLayer, LayerMask obstacleMask, Action<Collider> onRay)
    {
        var count = Physics.OverlapSphereNonAlloc(origin, range, CollidersBuffer, detectionLayer, QueryTriggerInteraction.Ignore);

        for (var i = 0; i < count; ++i)
        {
            var targetDir   = CollidersBuffer[i].transform.position - origin;
            var targetAngle = Mathf.Abs(Vector3.SignedAngle(targetDir, direction, up));

            if (targetAngle < angle / 2.0f)
            {
                if (Physics.Raycast(origin, targetDir, out var info, range, obstacleMask.value))
                {
                    if (CollidersBuffer[i].transform == info.transform)
                    {
                        onRay.Invoke(CollidersBuffer[i]);
                    }
                }
            }
        }

        if (count == CollidersBuffer.Length)
        {
            // Grow the buffer
            CollidersBuffer = new Collider[Mathf.RoundToInt(CollidersBuffer.Length * 1.5f)];
        }
    }

    public static void FieldOfView2D(Vector3 origin, Vector2 direction, float angle, float range, LayerMask detectionLayer, LayerMask obstacleMask, Action<Collider2D> onRay)
    {
        var count = Physics2D.OverlapCircleNonAlloc(origin, range, CollidersBuffer2D, detectionLayer);

        for (var i = 0; i < count; ++i)
        {
            var targetDir = CollidersBuffer2D[i].transform.position - origin;
            var targetAngle = Mathf.Abs(Vector2.SignedAngle(targetDir, direction));

            if (targetAngle < angle / 2.0f)
            {
                var info = Physics2D.Raycast(origin, targetDir, range, obstacleMask.value);
                if (info.collider != default)
                {
                    if (CollidersBuffer2D[i].transform == info.transform)
                    {
                        onRay.Invoke(CollidersBuffer2D[i]);
                    }
                }
            }
        }

        if (count == CollidersBuffer2D.Length)
        {
            // Grow the buffer
            CollidersBuffer2D = new Collider2D[Mathf.RoundToInt(CollidersBuffer2D.Length * 1.5f)];
        }
    }
}