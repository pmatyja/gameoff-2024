using System;
using UnityEngine;

public static class BoundsExtensions
{
    public static Bounds getBounds(this GameObject obj)
    {
        var bounds = new Bounds(obj.transform.position, Vector3.zero);

        if (obj.TryGetComponent<Renderer>(out var renderer))
        {
            bounds.Encapsulate(renderer.bounds);

            foreach (Transform child in obj.transform)
            {
                bounds.Encapsulate(child.gameObject.getBounds());
            }
        }

        return bounds;
    }

    public static Bounds getColliderBounds(this GameObject obj)
    {
        var bounds = new Bounds();

        if (obj.TryGetComponent<MeshCollider>(out var mesh))
        {
            bounds.Encapsulate(mesh.bounds);
        }
        else if (obj.TryGetComponent<BoxCollider>(out var box))
        {
            bounds.Encapsulate(box.bounds);
        }

        foreach (Transform child in obj.transform)
        {
            bounds.Encapsulate(child.gameObject.getColliderBounds());
        }

        return bounds;
    }

    public static Bounds getBounds(this GameObject obj, float minSize)
    {
        var bounds = obj.getBounds();

        return new Bounds
        (
            bounds.center,
            new Vector3
            (
                bounds.size.x.RoundUp(minSize),
                bounds.size.y.RoundUp(minSize),
                bounds.size.z.RoundUp(minSize)
            )
        );
    }
}