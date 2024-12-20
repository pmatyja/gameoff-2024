﻿namespace DeBroglie.Topo
{

    internal class TopoArray1D<T> : ITopoArray<T>
    {
        private readonly T[] values;

        public TopoArray1D(T[] values, ITopology topology)
        {
            this.Topology = topology;
            this.values = values;
        }

        public ITopology Topology { get; private set; }

        public T Get(int x, int y, int z)
        {
            return this.values[this.Topology.GetIndex(x, y, z)];
        }

        public T Get(int index)
        {
            return this.values[index];
        }
    }
}
