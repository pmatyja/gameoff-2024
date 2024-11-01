using DeBroglie.Models;
using DeBroglie.Topo;
using DeBroglie.Wfc;

namespace DeBroglie.Trackers
{
    public interface IQuadstateChanged
    {
        void Reset(SelectedChangeTracker tracker);

        void Notify(int index, Quadstate before, Quadstate after);
    }

    /// <summary>
    /// Runs a callback when the banned/selected status of tile changes with respect to a tileset.
    /// </summary>
    public class SelectedChangeTracker : ITracker
    {
        private readonly TilePropagator tilePropagator;

        private readonly WavePropagator wavePropagator;

        private readonly TileModelMapping tileModelMapping;

        // Indexed by tile topology
        private readonly int[] patternCounts;

        private readonly Quadstate[] values;

        private readonly TilePropagatorTileSet tileSet;

        private readonly IQuadstateChanged onChange;

        internal SelectedChangeTracker(TilePropagator tilePropagator, WavePropagator wavePropagator, TileModelMapping tileModelMapping, TilePropagatorTileSet tileSet, IQuadstateChanged onChange)
        {
            this.tilePropagator = tilePropagator;
            this.wavePropagator = wavePropagator;
            this.tileModelMapping = tileModelMapping;
            this.tileSet = tileSet;
            this.onChange = onChange;
            this.patternCounts = new int[tilePropagator.Topology.IndexCount];
            this.values = new Quadstate[tilePropagator.Topology.IndexCount];
        }

        private Quadstate GetQuadstateInner(int index)
        {
            var selectedPatternCount = this.patternCounts[index];

            this.tileModelMapping.GetTileCoordToPatternCoord(index, out var patternIndex, out _);

            var totalPatternCount = this.wavePropagator.Wave.GetPatternCount(patternIndex);

            if (totalPatternCount == 0)
            {
                return Quadstate.Contradiction;
            }
            else if (selectedPatternCount == 0)
            {
                return Quadstate.No;
            }
            else if (totalPatternCount == selectedPatternCount)
            {
                return Quadstate.Yes;
            }
            else
            {
                return Quadstate.Maybe;
            }
        }

        public Quadstate GetQuadstate(int index)
        {
            return this.values[index];
        }

        public bool IsSelected(int index)
        {
            return this.GetQuadstate(index).IsYes();
        }

        public void DoBan(int patternIndex, int pattern)
        {
            if(this.tileModelMapping.PatternCoordToTileCoordIndexAndOffset == null)
            {
                this.DoBan(patternIndex, pattern, patternIndex, 0);
            }
            else
            {
                foreach (var (_, index, offset) in this.tileModelMapping.PatternCoordToTileCoordIndexAndOffset.Get(patternIndex))
                {
                    this.DoBan(patternIndex, pattern, index, offset);
                }
            }
        }

        private void DoBan(int patternIndex, int pattern, int index, int offset)
        {
            var patterns = this.tileModelMapping.GetPatterns(this.tileSet, offset);
            if (patterns.Contains(pattern))
            {
                this.patternCounts[index] -= 1;
            }

            this.DoNotify(index);
        }

        public void Reset()
        {
            var wave = this.wavePropagator.Wave;
            foreach(var index in this.tilePropagator.Topology.GetIndices())
            {
                this.tileModelMapping.GetTileCoordToPatternCoord(index, out var patternIndex, out var offset);
                var patterns = this.tileModelMapping.GetPatterns(this.tileSet, offset);
                var count = 0;
                foreach (var p in patterns)
                {
                    if(patterns.Contains(p) && wave.Get(patternIndex, p))
                    {
                        count++;
                    }
                }

                this.patternCounts[index] = count;
                this.values[index] = this.GetQuadstateInner(index);
            }

            this.onChange.Reset(this);
        }


        public void UndoBan(int patternIndex, int pattern)
        {
            if (this.tileModelMapping.PatternCoordToTileCoordIndexAndOffset == null)
            {
                this.UndoBan(patternIndex, pattern, patternIndex, 0);
            }
            else
            {
                foreach (var (_, index, offset) in this.tileModelMapping.PatternCoordToTileCoordIndexAndOffset.Get(patternIndex))
                {
                    this.UndoBan(patternIndex, pattern, index, offset);
                }
            }
        }

        private void UndoBan(int patternIndex, int pattern, int index, int offset)
        {
            var patterns = this.tileModelMapping.GetPatterns(this.tileSet, offset);
            if (patterns.Contains(pattern))
            {
                this.patternCounts[index] += 1;
            }

            this.DoNotify(index);
        }

        private void DoNotify(int index)
        {
            var newValue = this.GetQuadstateInner(index);
            var oldValue = this.values[index];
            if (newValue != oldValue)
            {
                this.values[index] = newValue;
                this.onChange.Notify(index, oldValue, newValue);
            }
        }
    }
}
