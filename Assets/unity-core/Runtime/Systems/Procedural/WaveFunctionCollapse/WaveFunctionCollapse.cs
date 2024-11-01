using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveFunctionCollapse
{
    private ulong seed;

    public WaveFunctionCollapse(ulong seed)
    {
        this.seed = seed;
    }

    public bool Collapse<T>(IDictionary<Vector3Int, T> grid, Vector3Int position, WaveFunctionCollapseSettings settings) 
        where T : IWaveFunctionCollapseTile
    {
        var queue = new PriorityQueue<Vector3Int>(256, TileCoordinatesComparer.Default);
        var neighbours = new T[settings.Neighbours.Length];

        queue.Push(position);

        while (queue.TryPop(out var current))
        {
            this.ResolveConstraints(grid, settings, grid[current], neighbours, queue);
        }

        return true;
    }

    private void ResolveConstraints<T>(IDictionary<Vector3Int, T> grid, WaveFunctionCollapseSettings settings, T current, T[] neighbours, PriorityQueue<Vector3Int> queue) 
        where T : IWaveFunctionCollapseTile
    {
        // Rersolve if not resloved yet
        if (current.Resolved == false)
        {
            current.Resolved = true;

            var pair = current.Pairs.Skip(Rng.Range(ref this.seed, 0, current.Pairs.Count() - 1)).FirstOrDefault();
            if (pair != null)
            {
                current.Final = pair;
            }

            grid[current.Coordinates] = current;
        }

        // Get neighbours
        var neighboursCount = this.GetNeighbours(grid, settings, current.Coordinates, neighbours);

        // Queue unresolved neighbours
        for (var i = 0; i < neighboursCount; ++i)
        {
            if (neighbours[i].Resolved)
            {
                continue;
            }

            var neighbour = neighbours[i];

            neighbour.Pairs = neighbour.Pairs.Where(x =>
            {
                return 
                    x.CenterPrefab   == current.Final.CenterPrefab && 
                    x.CenterRotation == current.Final.CenterRotation;
            });

            if (neighbour.Pairs.Count() < 1)
            {
                neighbour.Resolved = true;
            }

            if (neighbour.Resolved == false)
            {
                queue.Push(neighbours[i].Coordinates);
            }

            grid[neighbour.Coordinates] = neighbour;
        }
    }

    private int GetNeighbours<T>(IDictionary<Vector3Int, T> grid, WaveFunctionCollapseSettings settings, Vector3Int position, T[] neighbours) 
        where T : IWaveFunctionCollapseTile
    {
        var count = 0;

        for (var i = 0; i < settings.Neighbours.Length; i++)
        {
            var neighbour = settings.Neighbours[i];

            if (grid.TryGetValue(neighbour + position, out neighbours[count]))
            {
                if (neighbours[count].Resolved)
                {
                    continue;
                }

                count++;
            }
        }

        return count;
    }
}