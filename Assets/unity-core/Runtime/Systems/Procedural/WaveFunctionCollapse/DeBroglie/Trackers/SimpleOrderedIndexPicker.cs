using DeBroglie.Wfc;
using System;
using System.Collections.Generic;

namespace DeBroglie.Trackers
{
    internal class SimpleOrderedIndexPicker : IIndexPicker, IFilteredIndexPicker
    {
        private bool[] mask;

        private int indices;

        private Wave wave;

        public SimpleOrderedIndexPicker()
        {
        }

        public void Init(WavePropagator wavePropagator)
        {
            this.wave = wavePropagator.Wave;

            this.mask = wavePropagator.Topology.Mask;

            this.indices = this.wave.Indicies;
        }

        public int GetRandomIndex(Func<double> randomDouble)
        {
            for (var i = 0; i < this.indices; i++)
            {
                if (this.mask != null && !this.mask[i])
                    continue;
                var c = this.wave.GetPatternCount(i);
                if (c <= 1)
                {
                    continue;
                }
                return i;
            }
            return -1;
        }

        public int GetRandomIndex(Func<double> randomDouble, IEnumerable<int> indices)
        {
            foreach(var i in indices)
            {
                var c = this.wave.GetPatternCount(i);
                if (c <= 1)
                {
                    continue;
                }
                return i;
            }
            return -1;
        }
    }
}
