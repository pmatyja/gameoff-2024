using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public abstract class VolumeNodeDictionary<TBlock, TNode>
    where TBlock : VolumeBlock<TNode>, new()
    where TNode : class, VolumeNode
{
    public readonly VolumeGrid<TBlock, TNode> Volume;
    public readonly ConcurrentDictionary<Vector3Int, TNode> Nodes = new();

    private ThreadLocal<TBlock> lastBlock = new ThreadLocal<TBlock>();

    protected VolumeNodeDictionary(VolumeGrid<TBlock, TNode> volume)
    {
        this.Volume = volume;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TNode GetNode(Vector3 position)
    {
        return GetNode(Vector3Int.FloorToInt(position));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TNode GetNode(Vector3Int position)
    {
        if (this.Volume.TryGet(position, out var block))
        {
            var nodeX = position.x.ToLocalIndex(this.Volume.BlockSize.y);
            var nodeY = position.y.ToLocalIndex(this.Volume.BlockSize.y);
            var nodeZ = position.z.ToLocalIndex(this.Volume.BlockSize.z);

            return block.Nodes[nodeX, nodeZ, nodeY];
        }

        return this.CreateNode(new Vector3Int(position.x, position.y, position.z));
    }

    protected abstract TNode CreateNode(Vector3Int coordinates);
}