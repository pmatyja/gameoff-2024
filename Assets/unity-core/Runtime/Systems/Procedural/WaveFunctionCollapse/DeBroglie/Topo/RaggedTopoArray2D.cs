using System.Linq;

namespace DeBroglie.Topo
{
    internal class RaggedTopoArray2D<T> : ITopoArray<T>
    {
        private readonly T[][] values;

        public RaggedTopoArray2D(T[][] values, bool periodic)
        {
            var height = values.Length;
            var width = values.Max(a => a.Length);
            this.Topology = new GridTopology(
                width,
                height,
                periodic);
            this.values = values;
        }

        public RaggedTopoArray2D(T[][] values, ITopology topology)
        {
            this.Topology = topology;
            this.values = values;
        }

        public ITopology Topology { get; private set; }

        public T Get(int x, int y, int z)
        {
            return this.values[y][x];
        }

        public T Get(int index)
        {
            int x, y, z;
            this.Topology.GetCoord(index, out x, out y, out z);
            return this.Get(x, y, z);
        }
    }
}
