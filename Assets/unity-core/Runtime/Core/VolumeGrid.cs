using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public abstract class VolumeGrid<TBlock, TNode>
    where TBlock : VolumeBlock<TNode>
    where TNode : VolumeNode
{
    public readonly ConcurrentDictionary<Vector3Int, TBlock> Blocks = new();
    public readonly Vector3Int BlockSize;

    private ThreadLocal<TBlock> lastBlock = new ThreadLocal<TBlock>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        this.Blocks.Clear();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGet(Vector3 position, out TBlock block)
    {
        return this.TryGet(Vector3Int.FloorToInt(position), out block);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGet(Vector3Int position, out TBlock block)
    {
        var coordinates = new Vector3Int
        (
            position.x.ToGlobalIndex(this.BlockSize.x),
            position.y.ToGlobalIndex(this.BlockSize.y),
            position.z.ToGlobalIndex(this.BlockSize.z)
        );

        block = this.lastBlock.Value;

        if (block.Coordinates == coordinates)
        {
            return true;
        }

        if (this.Blocks.TryGetValue(coordinates, out block))
        {
            this.lastBlock.Value = block;
            return true;
        }

        block = null;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddOrUpdate(Vector3 position, float range)
    {
        this.AddOrUpdate(new Bounds(position, new Vector3(range, range, range)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddOrUpdate(Bounds bounds)
    {
        this.EnumerateBlocks
        (
            bounds, 
            (blocks, position) =>
            {
                this.Blocks.AddOrUpdate
                (
                    position, 
                    position => this.CreateBlock(position),
                    (key, value) =>
                    {
                        value.IsDirty = true;
                        return value;
                    }
                );
            }
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetDirty(Vector3 position, float range)
    {
        this.SetDirty(new Bounds(position, new Vector3(range, range, range)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetDirty(Bounds bounds)
    {
        this.EnumerateBlocks
        (
            bounds,
            (blocks, position) =>
            {
                if (this.Blocks.TryGetValue(position, out var block))
                {
                    block.IsDirty = true;
                }
            }
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EnumerateBlocks(Vector3 position, float range, Action<ConcurrentDictionary<Vector3Int, TBlock>, Vector3Int> action)
    {
        this.EnumerateBlocks(new Bounds(position, new Vector3(range, range, range)), action);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EnumerateBlocks(Bounds bounds, Action<ConcurrentDictionary<Vector3Int, TBlock>, Vector3Int> action)
    {
        var minX = Mathf.FloorToInt(bounds.min.x).ToGlobalIndex(this.BlockSize.x);
        var maxX = Mathf.CeilToInt(bounds.max.x).ToGlobalIndex(this.BlockSize.x);

        var minY = Mathf.FloorToInt(bounds.min.y).ToGlobalIndex(this.BlockSize.y);
        var maxY = Mathf.CeilToInt(bounds.max.y).ToGlobalIndex(this.BlockSize.y);

        var minZ = Mathf.FloorToInt(bounds.min.z).ToGlobalIndex(this.BlockSize.z);
        var maxZ = Mathf.CeilToInt(bounds.max.z).ToGlobalIndex(this.BlockSize.z);

        for (var blockY = minY; blockY <= maxY; ++blockY)
        {
            for (var blockZ = minZ; blockZ <= maxZ; ++blockZ)
            {
                for (var blockX = minX; blockX <= maxX; ++blockX)
                {
                    action.Invoke(this.Blocks, new Vector3Int(blockX, blockY, blockZ));
                }
            }
        }
    }

    protected abstract TBlock CreateBlock(Vector3Int coordinates);
}