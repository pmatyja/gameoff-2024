using System.Runtime.CompilerServices;

public class ExpandableArray<T>
{
    public int Count;

    public T this[int index]
    {
        get => this.items[index];
        set => this.items[index] = value;
    }

    private T[] items;

    public ExpandableArray(int capacity = 16)
    {
        this.items = new T[capacity];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        this.Count = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Push(T item)
    {
        this.items[this.Count++] = item;

        if (this.Count == this.items.Length)
        {
            System.Array.Resize(ref this.items, this.Count * 2);
        }
    }
}