using System;
using System.Collections.Generic;
using System.Linq;
using DeBroglie;
using DeBroglie.Models;
using DeBroglie.Rot;
using DeBroglie.Topo;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(PrefabGroupSO), menuName = "Lavgine/Database.Prefab/Prefab Group")]
public class PrefabGroupSO : ScriptableObject
{
    public const int ItemsCount = sizeof(ulong) * 8;

    [Readonly]
    public string Id = Guid.NewGuid().ToString();

    [Range(1, 500)]
    public float GridSize = 5f;

    [Header("View")]
    public bool EditMode;
    public bool IsometricView = true;

    public List<PrefabPair> Pairs;

    public AdjacentModel BuildCartesian2dModel()
    {
        if (this.Pairs == null || this.Pairs.Count < 1)
        {
            this.Update();
        }

        if (this.Pairs.Count < 1)
        {
            Debug.LogError("No tiles in group. Please assign group to tiles");
        }

        var index = 0;
        var model = new AdjacentModel(DirectionSet.Cartesian2d);

        foreach (var pair in this.Pairs)
        {
            if (pair.CenterPrefab == null)
            {
                Debug.LogError($"Center Prefab is null at {index}");
                continue;
            }

            if (pair.AdjacentPrefab == null)
            {
                Debug.LogError($"Adjacent Prefab is null at {index}");
                continue;
            }

            this.AddTile(model, pair, 0, Direction.YPlus);
            this.AddTile(model, pair, 1, Direction.XPlus);
            this.AddTile(model, pair, 2, Direction.YMinus);
            this.AddTile(model, pair, 3, Direction.XMinus);

            index++;
        }

        return model;
    }

    public void Rebuild()
    {
        if (this.Pairs == null)
        {
            this.Pairs = new List<PrefabPair>();
        }

        this.Pairs.Clear();
        this.Update();
    }

    public void Update()
    {
        if (this.Pairs == null)
        {
            this.Pairs = new List<PrefabPair>();
        }

        PrefabSO.GeneratePrefabPairs(this, this.Pairs);
        this.Pairs = this.Pairs.Where(x => x.CenterPrefab != null || x.AdjacentPrefab != null).ToList();

        #if UNITY_EDITOR
            Debug.Log("Group tiles updated");
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        #endif
    }

    private void AddTile(AdjacentModel model, PrefabPair pair, int rotationOffset, Direction direction)
    {
        var center = new Tile(new RotatedTile
        {
            Tile = new Tile(pair.CenterPrefab),
            Rotation = new Rotation((pair.CenterRotation + rotationOffset) * pair.CenterPrefab.RotationStep)
        });

        var adjacent = new Tile(new RotatedTile
        {
            Tile = new Tile(pair.AdjacentPrefab),
            Rotation = new Rotation((pair.AdjacentRotation + rotationOffset) * pair.AdjacentPrefab.RotationStep)
        });

        model.AddAdjacency(center, adjacent, direction);
        model.SetFrequency(center, pair.CenterPrefab.Frequency);
        model.SetFrequency(adjacent, pair.AdjacentPrefab.Frequency);
    }
}