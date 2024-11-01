using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DeBroglie;
using DeBroglie.Constraints;
using DeBroglie.Rot;
using DeBroglie.Topo;
using DeBroglie.Wfc;
using UnityEngine;

public class WaveGenerator : MonoBehaviour
{
    public PrefabGroupSO Group;
    public ulong Seed;

    [Range(1, 100)]
    public int width = 3;

    [Range(1, 100)]
    public int height = 3;

    [Header("Generation")]
    public ModelConstraintAlgorithm ModelConstraintAlgorithm = ModelConstraintAlgorithm.Default;
    public IndexPickerType IndexPickerType = IndexPickerType.ArrayPriorityMinEntropy;

    [Header("Backtracking")]
    [Range(0, 5)]
    public int MaxBacktrackDepth;
    public BacktrackType BacktrackType = BacktrackType.Backtrack;
    public bool MemoizeIndices = true;

    [Header("Tiles")]
    public bool UniformFrequency;
    public TilePickerType TilePickerType = TilePickerType.Default;

    private Transform root;

    [ContextMenu("Clear")]
    public void Clear()
    {
        if (this.root == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            GameObject.Destroy(this.root.gameObject, 0.1f);
        }
        else
        {
            GameObject.DestroyImmediate(this.root.gameObject, true);
        }
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        this.Clear();

        this.root = this.transform.CreateChild("Root").transform;

        if (this.Group == null)
        {
            return;
        }

        var model = this.Group.BuildCartesian2dModel();

        var constraints = new ConcurrentDictionary<string, ITileConstraint>();

        foreach (var tile in model.Tiles)
        {
            var prefab = tile.Value as PrefabSO;

            if (tile.Value is RotatedTile rotatedTile)
            {
                prefab = rotatedTile.Tile.Value as PrefabSO;
            }

            if (prefab != null)
            {
                if (prefab.SeparationConstraint.Enabled && prefab.SeparationConstraint.MinDistance > 0)
                {
                    this.AddOrAppendConstraint<SeparationConstraint>(constraints, $"Separation:{prefab.SeparationConstraint}", constraint =>
                    {
                        if (constraint.Tiles == null)
                        {
                            constraint.Tiles = new HashSet<Tile>();
                        }

                        constraint.MinDistance = prefab.SeparationConstraint.MinDistance;
                        constraint.Tiles.Add(tile);
                    });
                }

                if (prefab.CountConstraint.Enabled && prefab.CountConstraint.Count > 0)
                {
                    this.AddOrAppendConstraint<CountConstraint>(constraints, $"Count:{prefab.CountConstraint}", constraint =>
                    {
                        if (constraint.Tiles == null)
                        {
                            constraint.Tiles = new HashSet<Tile>();
                        }

                        constraint.Count = prefab.CountConstraint.Count;
                        constraint.Comparison = prefab.CountConstraint.Comparison;
                        constraint.Tiles.Add(tile);
                    });
                }

                if (prefab.BorderConstraint.Enabled)
                {
                    this.AddOrAppendConstraint<BorderConstraint>(constraints, $"Border:{prefab.BorderConstraint}", constraint =>
                    {
                        if (constraint.Tiles == null)
                        {
                            constraint.Tiles = new Tile[0];
                        }

                        constraint.Sides = prefab.BorderConstraint.Sides;
                        constraint.ExcludeSides = prefab.BorderConstraint.ExcludeSides;
                        constraint.Ban = prefab.BorderConstraint.Ban;
                        constraint.InvertArea = prefab.BorderConstraint.InvertArea;
                        constraint.Tiles.Add(tile);
                    });
                }
            }
        }

        if (this.UniformFrequency)
        {
            model.SetUniformFrequency();
        }

        var topology = new GridTopology(this.width, this.height, periodic: false);
        var propagator = new TilePropagator(model, topology, new TilePropagatorOptions
        {
            Constraints = constraints.Values.ToArray(),
            RandomDouble = () =>
            {
                return Rng.Float(ref this.Seed);
            },

            ModelConstraintAlgorithm = this.ModelConstraintAlgorithm,
            IndexPickerType = this.IndexPickerType,

            MaxBacktrackDepth = this.MaxBacktrackDepth,
            BacktrackType = this.BacktrackType,
            MemoizeIndices = this.MemoizeIndices,

            TilePickerType = this.TilePickerType
        });

        if (propagator.Run() != DeBroglie.Resolution.Decided)
        {
            Debug.LogError($"Undecided: {propagator.ContradictionReason}, {propagator.ContradictionSource}");
            return;
        }

        var gridSize = this.Group.GridSize;
        var output = propagator.ToArray();

        for (var y = 0; y < topology.Height; y++)
        {
            for (var x = 0; x < topology.Width; x++)
            {
                var tile = output.Get(x, y);

                if (tile.Value is RotatedTile rotatedTile)
                {
                    if (rotatedTile.Tile.Value is PrefabSO prefab)
                    {
                        this.CreatePrefab(prefab, x, y, rotatedTile.Rotation.RotateCw, gridSize);
                    }
                    else
                    {
                        Debug.LogWarning($"Unknown prefab: {tile.Value}");
                    }
                }
                else if (tile.Value is PrefabSO prefab)
                {
                    this.CreatePrefab(prefab, x, y, 0, gridSize);
                }
                else
                {
                    Debug.LogWarning($"Unknown prefab: {tile.Value}");
                }
            }
        }
    }

    private void CreatePrefab(PrefabSO prefab, int x, int y, int rotation, float gridSize)
    {
        if (prefab != null)
        {
            this.root.CreateChild
            (
                prefab.Model,
                new Vector3(x * gridSize, 0f, y * gridSize),
                Quaternion.AngleAxis(rotation, prefab.RotationAxis),
                prefab.Scale,
                prefab.Pivot,
                prefab.Layer
            );
        }
        else
        {
            Debug.LogError($"Prefab is null");
        }
    }

    private ITileConstraint AddOrAppendConstraint<T>(ConcurrentDictionary<string, ITileConstraint> constraints, string key, Action<T> update) 
        where T : class, ITileConstraint, new()
    {
        return constraints.AddOrUpdate(key, 
            key =>
            {
                var value = new T();
                update.Invoke(value);
                return value;
            }, 
            (key, value) =>
            {
                update.Invoke(value as T);
                return value;
            });
    }
}