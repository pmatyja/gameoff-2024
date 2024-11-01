using System.Collections;
using System.Diagnostics;

namespace DeBroglie.Wfc
{

    /**
     * Wave is a fancy array that tracks various per-cell information.
     * Most importantly, it tracks possibilities - which patterns are possible to put
     * into which cells.
     * It has no notion of cell adjacency, cells are just referred to by integer index.
     */
    internal class Wave
    {
        private readonly int patternCount;

        // possibilities[index*patternCount + pattern] is true if we haven't eliminated putting
        // that pattern at that index.
        private readonly BitArray possibilities;

        private readonly int[] patternCounts;

        private readonly int indices;

        public Wave(int patternCount, int indices)
        {
            this.patternCount = patternCount;

            this.indices = indices;

            this.possibilities = new BitArray(indices * patternCount, true);

            this.patternCounts = new int[indices];
            for (var index = 0; index < indices; index++)
            {
                this.patternCounts[index] = patternCount;
            }
        }

        public int Indicies => this.indices;

        public bool Get(int index, int pattern)
        {
            return this.possibilities[index * this.patternCount + pattern];
        }

        public int GetPatternCount(int index)
        {
            return this.patternCounts[index];
        }

        // Returns true if there is a contradiction
        public bool RemovePossibility(int index, int pattern)
        {
            Debug.Assert(this.possibilities[index * this.patternCount + pattern] == true);
            this.possibilities[index * this.patternCount + pattern] = false;
            var c = --this.patternCounts[index];
            return c == 0;
        }

        public void AddPossibility(int index, int pattern)
        {
            Debug.Assert(this.possibilities[index * this.patternCount + pattern] == false);
            this.possibilities[index * this.patternCount + pattern] = true;
            this.patternCounts[index]++;
        }

        // TODO: This should respect mask. Maybe move out of Wave
        public double GetProgress()
        {
            // TODO: Use patternCount info?
            var c = 0;
            foreach(bool b in this.possibilities)
            {
                if (!b) c += 1;
            }
            // We're basically done when we've banned all but one pattern for each index
            return ((double)c) / (this.patternCount-1) / this.indices;
        }
    }
}
