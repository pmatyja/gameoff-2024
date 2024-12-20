﻿namespace DeBroglie.Topo
{

    internal class TopoArray2D<T> : ITopoArray<T>
    {
        private readonly T[,] values;

        public TopoArray2D(T[,] values, bool periodic)
        {
            this.Topology = new GridTopology(
                values.GetLength(0),
                values.GetLength(1),
                periodic);
            this.values = values;
        }

        public TopoArray2D(T[,] values, ITopology topology)
        {
            this.Topology = topology;
            this.values = values;
        }

        public ITopology Topology { get; private set; }

        public T Get(int x, int y, int z)
        {
            return this.values[x, y];
        }

        public T Get(int index)
        {
            int x, y, z;
            this.Topology.GetCoord(index, out x, out y, out z);
            return this.Get(x, y, z);
        }
    }
}
