using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class TransformExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Transform FindRecursively(this Transform transform, string name)
    {
        if (string.Equals(transform.name, name, StringComparison.OrdinalIgnoreCase))
        {
            return transform;
        }

        foreach (Transform child in transform)
        {
            if (string.Equals(child.name, name, StringComparison.OrdinalIgnoreCase))
            {
                return child;
            }

            var result = FindRecursively(child, name);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Transform FindBone(this Transform transform, Bone bone)
    {
        return FindRecursively(transform, bone.ToString());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GameObject CreateChild(this Transform parent, string name)
    {
        return CreateChild(parent, name, Vector3.zero);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GameObject CreateChild(this Transform parent, string name, Vector3 position)
    {
        var instance = new GameObject(name);

        instance.transform.SetParent(parent, false);
        instance.transform.localPosition = position;

        return instance;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GameObject CreateChild(this Transform parent, GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale, int layer = 0)
    {
        var instance = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);

        instance.name = $"{prefab.name} (Rot: {rotation.eulerAngles})";
        instance.transform.localPosition = position;
        instance.transform.localRotation = rotation;
        instance.transform.localScale = scale;
        instance.layer = layer;

        return instance;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GameObject CreateChild(this Transform parent, GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale, Vector3 pivot, int layer = 0)
    {
        var anchor  = CreateChild(parent, $"{prefab.name} (Rot: {rotation.eulerAngles}) - (Pivot)", position);
        
        anchor.transform.localRotation = rotation;

        CreateChild(anchor.transform, prefab, pivot, Quaternion.identity, scale, layer);

        return anchor;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GameObject CreateChild(this Transform parent, GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale, Bone bone)
    {
        var child = FindBone(parent, bone);
        if (child)
        {
            return CreateChild(child, prefab, position, rotation, scale);
        }

        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GameObject CreateChild(this Transform parent, PrefabSO prefab)
    {
        if (prefab == null)
        {
            return null;
        }
        
        return CreateChild(parent, prefab.Model, prefab.Position, prefab.Rotation, prefab.Scale, prefab.Bone);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GameObject CreateChild(this Transform parent, PrefabSO prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
        {
            return null;
        }
        
        return CreateChild(parent, prefab.Model, prefab.Position + position, rotation, prefab.Scale, prefab.Bone);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GameObject CreateChild(this Transform parent, PrefabSO prefab, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        if (prefab == null)
        {
            return null;
        }
        
        return CreateChild(parent, prefab.Model, prefab.Position + position, rotation, scale, prefab.Bone);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AttachChild(this Transform parent, GameObject instance, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        instance.transform.SetParent(parent);
        
        instance.transform.localPosition = position;
        instance.transform.localRotation = rotation;
        instance.transform.localScale = scale;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AttachChild(this Transform parent, GameObject instance, Vector3 position, Quaternion rotation, Vector3 scale, Bone bone)
    {
        var child = FindBone(parent, bone);
        if (child)
        {
            AttachChild(child, instance, position, rotation, scale);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AttachChild(this Transform parent, GameObject instance, PrefabSO prefab)
    {
        var child = FindBone(parent, prefab.Bone);
        if (child)
        {
            AttachChild(child, instance, prefab.Position, prefab.Rotation, prefab.Scale);
        }
    }
}