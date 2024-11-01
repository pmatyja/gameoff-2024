using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OutlineBuilder
{
    private static readonly IComparer<Vector3> Comparer = new OutlineBuilderComparer();
    private static readonly Quaternion RotateLeft = Quaternion.Euler(0, -90, 0);
    private static readonly Quaternion RotateRight = Quaternion.Euler(0, 90, 0);

    private static readonly Func<Vector3, Vector3>[] LeftTile =
    {
        position => new Vector3(position.x - 1.0f, position.y, position.z + 1.0f),
        position => new Vector3(position.x, position.y, position.z + 1.0f),
        position => position,
        position => new Vector3(position.x - 1.0f, position.y, position.z)
    };

    private static readonly Func<Vector3, Vector3>[] RightTile =
    {
        position => new Vector3(position.x, position.y, position.z + 1.0f),
        position => position,
        position => new Vector3(position.x - 1.0f, position.y, position.z),
        position => new Vector3(position.x - 1.0f, position.y, position.z + 1.0f)
    };

    public IEnumerable<Vector3> FromTiles(List<Vector3> tiles)
    {
        if (tiles.Any())
        {
            var visited = new HashSet<Vector3>(OutlineBuilderComparer.Default);
            var set = new HashSet<Vector3>(tiles.OrderBy(x => x, Comparer), OutlineBuilderComparer.Default);

            foreach (var tile in set)
            {
                if (visited.Contains(tile) == false)
                {
                    var points = this.BuildOutline(set, visited, tile);
                    if (points.Any())
                    {
                        return points;
                    }
                }
            }
        }

        return Enumerable.Empty<Vector3>();
    }

    private List<Vector3> BuildOutline(HashSet<Vector3> tiles, HashSet<Vector3> visited, Vector3 start)
    {
        var rotation = 0;
        var rotationsLeft = 3;
        var direction = new Vector3Int(0, 0, 1);

        var current = start;
        var points = new List<Vector3>();
        
        while ((points.Count < 1 || current != start))
        {
            if (tiles.Contains(LeftTile[rotation].Invoke(current)) &&
                tiles.Contains(RightTile[rotation].Invoke(current)) == false)
            {
                // Add point
                points.Add(current);
                
                // Add visited
                visited.Add(current);

                // Move
                current += direction;

                // Rotate one back
                direction = Vector3Int.RoundToInt(RotateLeft * direction);

                if (--rotation < 0)
                {
                    rotation = 3;
                }
            
                rotationsLeft = 3;
            }
            else
            {
                if (rotationsLeft == 0)
                {
                    break;
                }

                direction = Vector3Int.RoundToInt(RotateRight * direction);
                
                if (++rotation > 3)
                {
                    rotation = 0;
                }

                rotationsLeft--;
            }
        }

        return new List<Vector3>(points);
    }
}