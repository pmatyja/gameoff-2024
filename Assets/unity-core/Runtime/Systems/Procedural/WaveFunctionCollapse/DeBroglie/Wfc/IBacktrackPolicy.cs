using DeBroglie.Trackers;
using System;
using System.Collections.Generic;

namespace DeBroglie.Wfc
{
    internal interface IBacktrackPolicy
    {
        void Init(WavePropagator wavePropagator);

        /// <summary>
        /// 0  = Give up
        /// 1  = Backtrack
        /// >1 = Backjump
        /// </summary>
        int GetBackjump();
    }



    internal class ConstantBacktrackPolicy : IBacktrackPolicy
    {
        private readonly int amount;

        public ConstantBacktrackPolicy(int amount)
        {
            this.amount = amount;
        }

        public void Init(WavePropagator wavePropagator)
        {

        }

        public int GetBackjump()
        {
            return this.amount;
        }
    }

    // After 10 failed backtracks, backjumps by 4 (level 0) and repeats.
    // Each subsequent level backtrackts twice as far, but waits twice as long to trigger.
    // This means that after a backjump, we try exactly as hard the 2nd time as we did the first, including
    // trying smaller backjumps.
    // Whenever forward progress is made, all levels are reset.
    internal class PatienceBackjumpPolicy : IBacktrackPolicy, IChoiceObserver
    {
        private long counter;
        private int depth;
        private int maxDepth;
        private long start;

        private List<Level> levels;

        public void Init(WavePropagator wavePropagator)
        {
            wavePropagator.AddChoiceObserver(this);
            this.counter = 0;
            this.depth = 0;
            this.maxDepth = 0;
            this.start = 0;
        }

        public void MakeChoice(int index, int pattern)
        {
            this.depth++;
            if(this.depth > this.maxDepth)
            {
                this.maxDepth = this.depth;
                // Reset levels
                this.levels = null;
                this.start = this.counter;
            }
        }

        public void Backtrack()
        {
            this.counter++;
            this.depth--;
        }

        private Level CreateLevel(int level)
        {
            return new Level
            {
                depth = this.maxDepth - 4 * (int)Math.Pow(2, level),
                timeout = this.start + 10 * (long)Math.Pow(2, level)
            };
        }

        private void ResetLevel(int level)
        {
            this.levels[level].timeout = this.counter + 10 * (long)Math.Pow(2, level);
        }

        public int GetBackjump()
        {
            if (this.levels == null) this.levels = new List<Level>();
            // Find first non-expired level
            int i;
            for (i = 0; i < this.levels.Count; i++)
            {
                if (this.levels[i].timeout > this.counter)
                    break;
            }
            // Lazily add higher levels as needed
            if (this.levels.Count <= i)
            {
                this.levels.Add(this.CreateLevel(i));
            }
            if (i == 0)
            {
                return 1;
            }

            // Backjump to highest expired level

            // Reset any expired levels
            for (var j = 0; j < i; j++)
            {
                this.ResetLevel(j);
            }

            return this.depth - this.levels[i - 1].depth;
        }

        private class Level
        {
            public int depth;
            public long timeout;
        }
    }
}
