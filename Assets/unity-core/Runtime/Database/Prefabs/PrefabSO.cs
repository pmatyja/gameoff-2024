using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(PrefabSO), menuName = "Lavgine/Database.Prefab/Prefab")]
public class PrefabSO : ScriptableObject
{
    [Readonly]
    public string Id = Guid.NewGuid().ToString();
    public List<PrefabGroupSO> Groups;

    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale = Vector3.one;
    public Bone Bone;

    public LayerMask Layer;

    [Header("Properties")]
    public Vector3 Size = new(5.0f, 0.0f, 5.0f);
    public Vector3 Pivot = new(2.5f, 0.0f, 2.5f);

    [Readonly]
    public Vector3 RotationAxis = Vector3.up;

    [Readonly]
    [Range(45, 180)]
    public int RotationStep = 90;

    [Readonly]
    [Range(1, 4)]
    public int RotationCount = 4;

    public GameObject Model;

    [Header("Sockets")]
    public PrefabSocket TopLeft;
    public PrefabSocket TopRight;
    public PrefabSocket BottomLeft;
    public PrefabSocket BottomRight;

    [Header("Randomness")]
    [Range(0.0001f, 10.0f)]
    public float Frequency = 1.0f;

    [Header("Constraints")]
    public SeparationPrefabConstraint SeparationConstraint;
    public CountPrefabConstraint CountConstraint;
    public BorderPrefabConstraint BorderConstraint;

    [Header("Pairs Settings")]
    public bool IsometricView;

    public List<PrefabPair> Pairs;

    public Quaternion GetQuaternion(int rotation = 0)
    {
        return Quaternion.AngleAxis(rotation * this.RotationStep, this.RotationAxis);
    }

    public void OnUpdate()
    {
        if (this.Model == null)
        {
            return;
        }

        var instance = GameObject.Instantiate(this.Model);

        instance.hideFlags = HideFlags.HideAndDontSave;
        instance.transform.localPosition = this.Position;
        instance.transform.localRotation = this.Rotation;
        instance.transform.localScale = this.Scale;

        try
        {
            var bounds = instance.getBounds(1.0f);

            bounds.center = Vector3.zero;

            this.Size = bounds.size;
            this.Pivot = new Vector3
            (
                this.Size.x / 2.0f + bounds.center.x,
                0.0f,
                this.Size.z / 2.0f + bounds.center.z
            );
        }
        finally
        {
            GameObject.DestroyImmediate(instance);
        }
    }

    public void UpdatePrefabPairs()
    {
        PrefabSO.GeneratePrefabPairs(this, PrefabSO.GetPrefabs(this.Groups), this.Pairs);
        EditorOnly.SaveAssets();
        EditorOnly.SetDirty(this);
    }

    public static IEnumerable<PrefabSO> GetPrefabs(IEnumerable<PrefabGroupSO> groups)
    {
        return EditorOnly
            .FindAssets<PrefabSO>("t:" + typeof(PrefabSO).Name, "Assets/Resources/Prefabs/")
            .Where(x => x.Groups.Any(g => groups.Contains(g)));
    }

    public static IEnumerable<PrefabSO> GetPrefabs(PrefabGroupSO group)
    {
        return EditorOnly
            .FindAssets<PrefabSO>("t:" + typeof(PrefabSO).Name, "Assets/Resources/Prefabs/")
            .Where(x => x.Groups.Contains(group));
    }

    public static void GeneratePrefabPairs(PrefabGroupSO group, List<PrefabPair> results)
    {
        var prefabs = PrefabSO.GetPrefabs(group);

        foreach (var prefab in prefabs)
        {
            PrefabSO.GeneratePrefabPairs(prefab, prefabs, results);
        }
    }

    public static void GeneratePrefabPairs(IEnumerable<PrefabGroupSO> groups, List<PrefabPair> results)
    {
        var prefabs = PrefabSO.GetPrefabs(groups);

        foreach (var prefab in prefabs)
        {
            PrefabSO.GeneratePrefabPairs(prefab, prefabs, results);
        }
    }

    public static void GeneratePrefabPairs(PrefabSO center, IEnumerable<PrefabSO> prefabs, List<PrefabPair> results)
    {
        foreach (var prefab in prefabs)
        {
            if (PrefabSocket.Match(center.TopLeft, prefab.BottomRight))           TryAppend(results, new PrefabPair(center, 0, prefab, 0, Vector3Int.forward));
            if (PrefabSocket.Match(center.TopLeft, prefab.TopRight, true))        TryAppend(results, new PrefabPair(center, 0, prefab, 1, Vector3Int.forward));
            if (PrefabSocket.Match(center.TopLeft, prefab.TopLeft, true))         TryAppend(results, new PrefabPair(center, 0, prefab, 2, Vector3Int.forward));
            if (PrefabSocket.Match(center.TopLeft, prefab.BottomLeft))            TryAppend(results, new PrefabPair(center, 0, prefab, 3, Vector3Int.forward));

            if (PrefabSocket.Match(center.TopRight, prefab.BottomRight))          TryAppend(results, new PrefabPair(center, 3, prefab, 0, Vector3Int.right));
            if (PrefabSocket.Match(center.TopRight, prefab.TopRight, true))       TryAppend(results, new PrefabPair(center, 3, prefab, 1, Vector3Int.right));
            if (PrefabSocket.Match(center.TopRight, prefab.TopLeft, true))        TryAppend(results, new PrefabPair(center, 3, prefab, 2, Vector3Int.right));
            if (PrefabSocket.Match(center.TopRight, prefab.BottomLeft))           TryAppend(results, new PrefabPair(center, 3, prefab, 3, Vector3Int.right));

            if (PrefabSocket.Match(center.BottomLeft, prefab.BottomRight, true))  TryAppend(results, new PrefabPair(center, 1, prefab, 0, Vector3Int.left));
            if (PrefabSocket.Match(center.BottomLeft, prefab.TopRight))           TryAppend(results, new PrefabPair(center, 1, prefab, 1, Vector3Int.left));
            if (PrefabSocket.Match(center.BottomLeft, prefab.TopLeft))            TryAppend(results, new PrefabPair(center, 1, prefab, 2, Vector3Int.left));
            if (PrefabSocket.Match(center.BottomLeft, prefab.BottomLeft, true))   TryAppend(results, new PrefabPair(center, 1, prefab, 3, Vector3Int.left));

            if (PrefabSocket.Match(center.BottomRight, prefab.BottomRight, true)) TryAppend(results, new PrefabPair(center, 2, prefab, 0, Vector3Int.back));
            if (PrefabSocket.Match(center.BottomRight, prefab.TopRight))          TryAppend(results, new PrefabPair(center, 2, prefab, 1, Vector3Int.back));
            if (PrefabSocket.Match(center.BottomRight, prefab.TopLeft))           TryAppend(results, new PrefabPair(center, 2, prefab, 2, Vector3Int.back));
            if (PrefabSocket.Match(center.BottomRight, prefab.BottomLeft, true))  TryAppend(results, new PrefabPair(center, 2, prefab, 3, Vector3Int.back));
        }
    }

    private static void TryAppend(List<PrefabPair> results, PrefabPair pair)
    {
        if (results.Contains(pair, PrefabPair.DefaultEqualityComparer))
        {
            return;
        }

        results.Add(pair);
    }
}
