using System;
using System.Collections.Generic;

public class PriorityQueue<T>
{
    public int Count => this.count;
    public bool NotEmpty => this.count > 0;

    public T Top
    {
        get
        {
            if (!this.isHeap)
            {
                this.Heapify();
            }

            return this.heap[0];
        }
    }

    private T[] heap;
    private int count;
    private bool isHeap;
    
    private const int DefaultCapacity = 6;
    
    private readonly IComparer<T> comparer;

    public PriorityQueue(int capacity, IComparer<T> comparer)
    {
        this.heap = new T[capacity > 0 ? capacity : DefaultCapacity];
        this.count = 0;
        this.comparer = comparer;
    }

    public void Clear()
    {
        this.count = 0;
    }

    public bool Contains(T item)
    {
        for (var i = 0; i < this.count; ++i)
        {
            if (this.heap[i].Equals(item))
            {
                return true;
            }
        }

        return false;
    }

    public void Push(T value)
    {
        if (this.count == this.heap.Length)
        {
            Array.Resize(ref this.heap, this.count * 2);
        }

        if (this.isHeap)
        {
            this.SiftUp(this.count, ref value, 0);
        }
        else
        {
            this.heap[this.count] = value;
        }

        this.count++;
    }

    public void Pop()
    {
        if (!this.isHeap)
        {
            this.Heapify();
        }

        if (this.count > 0)
        {
            --this.count;

            var x = this.heap[this.count];
            var index = this.SiftDown(0);
            this.SiftUp(index, ref x, 0);
            this.heap[this.count] = default;
        }
    }

    public bool TryPop(out T result)
    {
        if (this.Count > 0)
        {
            result = this.Top;
            this.Pop();
            return true;
        }

        result = default;
        return false;
    }

    private int SiftDown(int index)
    {
        var parent = index;
        var leftChild = HeapLeftChild(parent);

        while (leftChild < this.count)
        {
            var rightChild = HeapRightFromLeft(leftChild);
            var bestChild =
                (rightChild < this.count && this.comparer.Compare(this.heap[rightChild], this.heap[leftChild]) < 0) ?
                rightChild : leftChild;

            this.heap[parent] = this.heap[bestChild];

            parent = bestChild;
            leftChild = HeapLeftChild(parent);
        }

        return parent;
    }

    private void SiftUp(int index, ref T x, int boundary)
    {
        while (index > boundary)
        {
            var parent = HeapParent(index);
            
            if (this.comparer.Compare(this.heap[parent], x) > 0)
            {
                this.heap[index] = this.heap[parent];
                index = parent;
            }
            else
            {
                break;
            }
        }
        this.heap[index] = x;
    }

    private void Heapify()
    {
        if (!this.isHeap)
        {
            for (var i = this.count/2 - 1; i >= 0; --i)
            {
                var x = this.heap[i];
                var index = this.SiftDown(i);
                this.SiftUp(index, ref x, i);
            }
            this.isHeap = true;
        }
    }

    private static int HeapParent(int i)
    {
        return (i - 1) / 2;
    }

    private static int HeapLeftChild(int i)
    {
        return (i * 2) + 1;
    }

    private static int HeapRightFromLeft(int i)
    {
        return i + 1;
    }
}