﻿namespace DeBroglie.Topo
{
    internal class TopoArrayConstant<T> : ITopoArray<T>
    {
        private readonly T value;

        public TopoArrayConstant(T value, ITopology topology)
        {
            this.Topology = topology;
            this.value = value;
        }

        public ITopology Topology { get; private set; }

        public T Get(int x, int y, int z)
        {
            return this.value;
        }

        public T Get(int index)
        {
            return this.value;
        }
    }
}
