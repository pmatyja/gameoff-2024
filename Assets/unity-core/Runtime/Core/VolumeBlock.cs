using System;
using UnityEngine;

public abstract class VolumeBlock<TNode> where TNode : VolumeNode
{
    public readonly Vector3Int Coordinates;
    public readonly Vector3Int Size;
    public readonly Bounds Bounds;
    public readonly TNode[,,] Nodes;
    public bool IsDirty;

    public VolumeBlock(Func<Vector3Int, TNode> createNode, Vector3Int coordinates, Vector3Int size)
    {
        this.Coordinates = coordinates;
        this.Size = size;
        this.Bounds = new Bounds
        (
            new Vector3
            (
                this.Coordinates.x * this.Size.x + (this.Size.x * 0.5f) - 0.5f,
                this.Coordinates.y * this.Size.y + (this.Size.y * 0.5f),
                this.Coordinates.z * this.Size.z + (this.Size.z * 0.5f) - 0.5f
            ),
            this.Size
        );
        this.Nodes = new TNode[this.Size.x, this.Size.z, this.Size.y];
        this.IsDirty = true;

        for (var y = 0; y < this.Size.y; ++y)
        {
            for (var z = 0; z < this.Size.z; ++z)
            {
                for (var x = 0; x < this.Size.x; ++x)
                {
                    this.Nodes[x, z, y] = createNode.Invoke
                    (
                        new Vector3Int
                        (
                            coordinates.x * this.Size.x + x, 
                            coordinates.y * this.Size.y + y, 
                            coordinates.z * this.Size.z + z
                        )
                    );
                }
            }
        }
    }

    public TNode GetNode(Vector3Int coordinates)
    {
        coordinates = new Vector3Int
        (
            coordinates.x.ToLocalIndex(this.Size.x),
            coordinates.y.ToLocalIndex(this.Size.y),
            coordinates.z.ToLocalIndex(this.Size.z)
        );

        return this.Nodes[coordinates.x, coordinates.z, coordinates.y];
    }

    public void OnDrawGizmos(bool showBlockBounds, bool showTraversable, bool showCollisions)
    {
        if (showBlockBounds)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(this.Bounds.center, this.Bounds.size);
        }

        for (var y = 0; y < this.Size.y; ++y)
        {
            for (var z = 0; z < this.Size.z; ++z)
            {
                for (var x = 0; x < this.Size.x; ++x)
                {
                    this.Nodes[x, z, y].OnDrawGizmos(showTraversable, showCollisions);
                }
            }
        }
    }

    public override string ToString()
    {
        return this.Coordinates.ToString();
    }
}