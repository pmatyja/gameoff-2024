using DeBroglie.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeBroglie.Trackers
{
    /// <summary>
    /// Tracks recently changed indices.
    /// This is useful for efficiently dealing with a small amount of changes to a large topology.
    /// Note that the list of changed indices can contain duplicates.
    /// The behaviour during the first call to GetChangedIndices() is undefined and subject to change.
    /// </summary>
    public class ChangeTracker : ITracker
    {
        private readonly TileModelMapping tileModelMapping;
        private readonly int indexCount;

        // Using pattern topology
        private List<int> changedIndices;

        // Double buffering
        private List<int> changedIndices2;

        private int generation;

        private int[] lastChangedGeneration;

        internal ChangeTracker(TileModelMapping tileModelMapping)
            :this(tileModelMapping, tileModelMapping.PatternTopology.IndexCount)
        {
        }

        internal ChangeTracker(TileModelMapping tileModelMapping, int indexCount)
        {
            this.tileModelMapping = tileModelMapping;
            this.indexCount = indexCount;
        }

        public int ChangedCount => this.changedIndices.Count;

        /// <summary>
        /// Returns the set of indices that have been changed since the last call.
        /// </summary>
        public IEnumerable<int> GetChangedIndices()
        {
            var currentChangedIndices = this.changedIndices;

            // Switch over double buffering
            (this.changedIndices, this.changedIndices2) = (this.changedIndices2, this.changedIndices);
            this.changedIndices.Clear();
            this.generation++;

            if(this.generation == int.MaxValue)
            {
                throw new Exception($"Change Tracker doesn't support more than {int.MaxValue} executions");
            }

            if (this.tileModelMapping.PatternCoordToTileCoordIndexAndOffset == null)
            {
                return currentChangedIndices;
            }
            else
            {
                return currentChangedIndices.SelectMany(i => this.tileModelMapping.PatternCoordToTileCoordIndexAndOffset.Get(i).Select(x => x.Item2));
            }
        }

        void ITracker.DoBan(int index, int pattern)
        {
            var g = this.lastChangedGeneration[index];
            if(g != this.generation)
            {
                this.lastChangedGeneration[index] = this.generation;
                this.changedIndices.Add(index);
            }
        }

        void ITracker.Reset()
        {
            this.changedIndices = new List<int>();
            this.changedIndices2 = new List<int>();
            this.lastChangedGeneration = new int[this.indexCount];
            this.generation = 1;
        }

        void ITracker.UndoBan(int index, int pattern)
        {
            var g = this.lastChangedGeneration[index];
            if (g != this.generation)
            {
                this.lastChangedGeneration[index] = this.generation;
                this.changedIndices.Add(index);
            }
        }
    }
}
