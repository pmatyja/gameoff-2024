using DeBroglie.Topo;
using DeBroglie.Wfc;
using System;
using System.Collections.Generic;

namespace DeBroglie.Trackers
{
    class DirtyIndexPicker : IIndexPicker, ITracker
    {
        private readonly IFilteredIndexPicker filteredIndexPicker;
        private readonly HashSet<int> dirtyIndices;
        private readonly ITopoArray<int> cleanPatterns;

        public DirtyIndexPicker(IFilteredIndexPicker filteredIndexPicker, ITopoArray<int> cleanPatterns)
        {
            this.dirtyIndices = new HashSet<int>();
            this.filteredIndexPicker = filteredIndexPicker;
            this.cleanPatterns = cleanPatterns;
        }

        public void Init(WavePropagator wavePropagator)
        {
            this.filteredIndexPicker.Init(wavePropagator);
            wavePropagator.AddTracker(this);
        }

        public void DoBan(int index, int pattern)
        {
            var clean = this.cleanPatterns.Get(index);
            if (clean == pattern) this.dirtyIndices.Add(index);
        }

        public int GetRandomIndex(Func<double> randomDouble)
        {
            return this.filteredIndexPicker.GetRandomIndex(randomDouble, this.dirtyIndices);
        }

        public void Reset()
        {
            this.dirtyIndices.Clear();
        }

        public void UndoBan(int index, int pattern)
        {
            // Doesn't undo dirty, it's too annoying to track
        }
    }
}
