using DeBroglie.Models;
using DeBroglie.Topo;
using DeBroglie.Wfc;
using System.Runtime.CompilerServices;

namespace DeBroglie.Trackers
{
    /// <summary>
    /// Tracks the banned/selected status of each tile with respect to a tileset.
    /// </summary>
    public class SelectedTracker : ITracker
    {
        private readonly TilePropagator tilePropagator;

        private readonly WavePropagator wavePropagator;

        private readonly TileModelMapping tileModelMapping;

        // Indexed by tile topology
        private readonly int[] patternCounts;

        private readonly TilePropagatorTileSet tileSet;

        internal SelectedTracker(TilePropagator tilePropagator, WavePropagator wavePropagator, TileModelMapping tileModelMapping, TilePropagatorTileSet tileSet)
        {
            this.tilePropagator = tilePropagator;
            this.wavePropagator = wavePropagator;
            this.tileModelMapping = tileModelMapping;
            this.tileSet = tileSet;
            this.patternCounts = new int[tilePropagator.Topology.IndexCount];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Quadstate GetQuadstate(int index)
        {
            var selectedPatternCount = this.patternCounts[index];
            if (selectedPatternCount == 0)
                return Quadstate.No;

            this.tileModelMapping.GetTileCoordToPatternCoord(index, out var patternIndex, out _);

            var totalPatternCount = this.wavePropagator.Wave.GetPatternCount(patternIndex);
            if (totalPatternCount == selectedPatternCount)
            {
                return Quadstate.Yes;
            }
            return Quadstate.Maybe;
        }

        public bool IsSelected(int index)
        {
            return this.GetQuadstate(index).IsYes();
        }

        void ITracker.DoBan(int patternIndex, int pattern)
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
        }

        void ITracker.Reset()
        {
            var wave = this.wavePropagator.Wave;
            foreach(var index in this.tilePropagator.Topology.GetIndices())
            {
                this.tileModelMapping.GetTileCoordToPatternCoord(index, out var patternIndex, out var offset);
                var patterns = this.tileModelMapping.GetPatterns(this.tileSet, offset);
                var count = 0;
                foreach (var p in patterns)
                {
                    if(wave.Get(patternIndex, p))
                    {
                        count++;
                    }
                }

                this.patternCounts[index] = count;
            }
        }


        void ITracker.UndoBan(int patternIndex, int pattern)
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
        }
    }
}
