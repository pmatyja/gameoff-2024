using DeBroglie.Wfc;
using System;

namespace DeBroglie.Trackers
{
    /// <summary>
    /// An <see cref="IRandomPicker"/> that picks cells based on min entropy heuristic.
    /// It's slower than <see cref="EntropyTracker"/> but supports two extra features:
    /// * The frequencies can be set on a per cell basis.
    /// * In addition to frequency, priority can be set. Only tiles of the highest priority for a given cell are considered available.
    /// </summary>
    internal class ArrayPriorityEntropyTracker : ITracker, IIndexPicker, IPatternPicker
    {
        private readonly WeightSetCollection weightSetCollection;

        // Track some useful per-cell values
        private EntropyValues[] entropyValues;

        private bool[] mask;

        private int indices;

        private Wave wave;

        public ArrayPriorityEntropyTracker(WeightSetCollection weightSetCollection)
        {
            this.weightSetCollection = weightSetCollection;

        }

        public void Init(WavePropagator wavePropagator)
        {
            this.mask = wavePropagator.Topology.Mask;
            this.wave = wavePropagator.Wave;
            this.indices = this.wave.Indicies;
            this.entropyValues = new EntropyValues[this.indices];

            this.Reset();
            wavePropagator.AddTracker(this);
        }

        // Don't run init twice
        void IPatternPicker.Init(WavePropagator wavePropagator)
        {

        }


        public void DoBan(int index, int pattern)
        {
            var frequencySet = this.weightSetCollection.Get(index);
            if (this.entropyValues[index].Decrement(frequencySet.priorityIndices[pattern], frequencySet.frequencies[pattern], frequencySet.plogp[pattern]))
            {
                this.PriorityReset(index);
            }
        }

        public void Reset()
        {
            // TODO: Perf boost by assuming wave is truly fresh?
            EntropyValues initial;
            initial.PriorityIndex = 0;
            initial.PlogpSum = 0;
            initial.Sum = 0;
            initial.Count = 0;
            initial.Entropy = 0;
            for (var index = 0; index < this.indices; index++)
            {
                this.entropyValues[index] = initial;
                if (this.weightSetCollection.Get(index) != null)
                {
                    this.PriorityReset(index);
                }
            }
        }

        // The priority has just changed, recompute
        private void PriorityReset(int index)
        {
            var frequencySet = this.weightSetCollection.Get(index);
            ref var v = ref this.entropyValues[index];
            v.PlogpSum = 0;
            v.Sum = 0;
            v.Count = 0;
            v.Entropy = 0;
            while (v.PriorityIndex < frequencySet.groups.Length)
            {
                ref var g = ref frequencySet.groups[v.PriorityIndex];
                for (var i = 0; i < g.patternCount; i++)
                {
                    if (this.wave.Get(index, g.patterns[i]))
                    {
                        v.Sum += g.frequencies[i];
                        v.PlogpSum += g.plogp[i];
                        v.Count += 1;
                    }
                }
                if(v.Count == 0)
                {
                    // Try again with the next priorityIndex
                    v.PriorityIndex++;
                    continue;
                }
                v.RecomputeEntropy();
                return;
            }
        }

        public void UndoBan(int index, int pattern)
        {
            var frequencySet = this.weightSetCollection.Get(index);
            if (this.entropyValues[index].Increment(frequencySet.priorityIndices[pattern], frequencySet.frequencies[pattern], frequencySet.plogp[pattern]))
            {
                this.PriorityReset(index);
            }
        }

        // Finds the cells with minimal entropy (excluding 0, decided cells)
        // and picks one randomly.
        // Returns -1 if every cell is decided.
        public int GetRandomIndex(Func<double> randomDouble)
        {
            var selectedIndex = -1;
            // TODO: At the moment this is a linear scan, but potentially
            // could use some data structure
            var minPriorityIndex = int.MaxValue;
            var minEntropy = double.PositiveInfinity;
            var countAtMinEntropy = 0;
            for (var i = 0; i < this.indices; i++)
            {
                if (this.mask != null && !this.mask[i])
                    continue;
                var c = this.wave.GetPatternCount(i);
                var pi = this.entropyValues[i].PriorityIndex;
                var e = this.entropyValues[i].Entropy;
                if (c <= 1)
                {
                    continue;
                }
                else if (pi < minPriorityIndex || (pi == minPriorityIndex && e < minEntropy))
                {
                    countAtMinEntropy = 1;
                    minEntropy = e;
                    minPriorityIndex = pi;
                }
                else if (pi == minPriorityIndex && e == minEntropy)
                {
                    countAtMinEntropy++;
                }
            }
            var n = (int)(countAtMinEntropy * randomDouble());

            for (var i = 0; i < this.indices; i++)
            {
                if (this.mask != null && !this.mask[i])
                    continue;
                var c = this.wave.GetPatternCount(i);
                var pi = this.entropyValues[i].PriorityIndex;
                var e = this.entropyValues[i].Entropy;
                if (c <= 1)
                {
                    continue;
                }
                else if (pi == minPriorityIndex && e == minEntropy)
                {
                    if (n == 0)
                    {
                        selectedIndex = i;
                        break;
                    }
                    n--;
                }
            }
            return selectedIndex;
        }

        public int GetRandomPossiblePatternAt(int index, Func<double> randomDouble)
        {
            var frequencySet = this.weightSetCollection.Get(index);
            ref var g = ref frequencySet.groups[this.entropyValues[index].PriorityIndex];
            return RandomPickerUtils.GetRandomPossiblePattern(this.wave, randomDouble, index, g.frequencies, g.patterns);
        }

        /**
          * Struct containing the values needed to compute the entropy of all the cells.
          * This struct is updated every time the cell is changed.
          * p'(pattern) is equal to Frequencies[pattern] if the pattern is still possible, otherwise 0.
          */
        private struct EntropyValues
        {
            public int PriorityIndex;
            public double PlogpSum;     // The sum of p'(pattern) * log(p'(pattern)).
            public double Sum;          // The sum of p'(pattern).
            public int Count;
            public double Entropy;      // The entropy of the cell.

            public void RecomputeEntropy()
            {
                this.Entropy = Math.Log(this.Sum) - this.PlogpSum / this.Sum;
            }

            public bool Decrement(int priorityIndex, double p, double plogp)
            {
                if (priorityIndex == this.PriorityIndex)
                {
                    this.PlogpSum -= plogp;
                    this.Sum -= p;
                    this.Count--;
                    if (this.Count == 0)
                    {
                        this.PriorityIndex++;
                        return true;
                    }

                    this.RecomputeEntropy();
                }
                return false;
            }

            public bool Increment(int priorityIndex, double p, double plogp)
            {
                if (priorityIndex == this.PriorityIndex)
                {
                    this.PlogpSum += plogp;
                    this.Sum += p;
                    this.Count++;
                    this.RecomputeEntropy();
                }
                if (priorityIndex < this.PriorityIndex)
                {
                    this.PriorityIndex = priorityIndex;
                    return true;
                }
                return false;
            }
        }
    }
}
