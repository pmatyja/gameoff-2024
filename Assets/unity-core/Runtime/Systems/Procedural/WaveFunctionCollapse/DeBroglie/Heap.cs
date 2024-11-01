using System;
using System.Collections.Generic;
using System.Linq;

namespace DeBroglie
{
    internal interface IHeapNode<TKey> where TKey : IComparable<TKey>
    {
        int HeapIndex { get; set; }

        TKey Key { get; }
    }

    /// <summary>
    /// Implements a basic min-key heap.
    /// </summary>
    internal class Heap<T, TKey> where T : IHeapNode<TKey> where TKey : IComparable<TKey>
    {
        T[] data;
        int size;

        private static int Parent(int i) => (i - 1) >> 1;

        private static int Left(int i) => (i << 1) + 1;
        private static int Right(int i) => (i << 1) + 2;

        public Heap()
        {
            this.data = new T[0];
            this.size = 0;
        }

        public Heap(int capacity)
        {
            this.data = new T[capacity];
            this.size = 0;
        }

        public Heap(T[] items)
        {
            this.data = new T[items.Length];
            this.size = this.data.Length;
            Array.Copy(items, this.data, this.data.Length);
            for (var i = 0; i< this.size;i++)
            {
                this.data[i].HeapIndex = i;
            }

            this.Heapify();
        }

        public Heap(IEnumerable<T> items)
        {
            this.data = items.ToArray();
            this.size = this.data.Length;
            for (var i = 0; i < this.size; i++)
            {
                this.data[i].HeapIndex = i;
            }

            this.Heapify();
        }

        public int Count => this.size;

        public T Peek()
        {
            if (this.size == 0)
                throw new Exception("Heap is empty");
            return this.data[0];
        }

        public void Heapify()
        {
            for (var i = Parent(this.size); i >= 0; i--)
            {
                this.Heapify(i);
            }
        }

        private void Heapify(int i)
        {
            var ip = this.data[i].Key;
            var smallest = i;
            var smallestP = ip;
            var l = Left(i);
            if (l < this.size)
            {
                var lp = this.data[l].Key;
                if (lp.CompareTo(smallestP) < 0)
                {
                    smallest = l;
                    smallestP = lp;
                }
            }
            var r = Right(i);
            if (r < this.size)
            {
                var rp = this.data[r].Key;
                if (rp.CompareTo(smallestP) < 0)
                {
                    smallest = r;
                    smallestP = rp;
                }
            }
            if(i == smallest)
            {
                this.data[i].HeapIndex = i;
            }
            else
            {
                (this.data[i], this.data[smallest]) = (this.data[smallest], this.data[i]);
                this.data[i].HeapIndex = i;
                this.Heapify(smallest);
            }
        }

        public void DecreasedKey(T item)
        {
            var i = item.HeapIndex;
            var priority = item.Key;
            while (true)
            {
                if (i == 0)
                {
                    item.HeapIndex = i;
                    return;
                }

                var p = Parent(i);
                var parent = this.data[p];
                var parentP = parent.Key;

                if (parentP.CompareTo(priority) > 0)
                {
                    (this.data[p], this.data[i]) = (this.data[i], this.data[p]);
                    parent.HeapIndex = i;
                    i = p;
                    continue;
                }
                else
                {
                    item.HeapIndex = i;
                    return;
                }
            }
        }

        public void IncreasedKey(T item)
        {
            this.Heapify(item.HeapIndex);
        }

        public void ChangedKey(T item)
        {
            this.DecreasedKey(item);
            this.IncreasedKey(item);
        }

        public void Insert(T item)
        {
            if(this.data.Length == this.size)
            {
                var data2 = new T[this.size * 2];
                Array.Copy(this.data, data2, this.size);
                this.data = data2;
            }

            this.data[this.size] = item;
            item.HeapIndex = this.size;
            this.size++;
            this.DecreasedKey(item);
        }

        public void Delete(T item)
        {
            var i = item.HeapIndex;
            if (i == this.size - 1)
            {
                this.size--;
            }
            else
            {
                item = this.data[i] = this.data[this.size - 1];
                item.HeapIndex = i;
                this.size--;
                this.IncreasedKey(item);
                this.DecreasedKey(item);
            }
        }

        public void Clear()
        {
            this.size = 0;
        }
    }
}
