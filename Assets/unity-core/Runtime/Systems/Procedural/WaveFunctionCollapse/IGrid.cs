using System.Collections.Generic;

public interface IGrid<T> : IComparer<T>
{
    int Width { get; }
    int Height { get; }

    T this[int x, int y, int z] { get; set; }
}