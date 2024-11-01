using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DeBroglie.Topo;

namespace DeBroglie.Wfc
{
    /// <summary>
    /// Implements pattern adjacency propagation using the arc consistency 4 algorithm.
    /// 
    /// Roughly speaking, this algorith keeps a count for each cell/pattern/direction of the "support",
    /// i.e. how many possible cells could adjoin that particular pattern.
    /// This count can be straightforwardly updated, and when it drops to zero, we know that that cell/pattern is not possible, and can be banned.
    /// </summary>
    internal class Ac4PatternModelConstraint : IPatternModelConstraint
    {
        // From model
        private int[][][] propagatorArray;
        private int patternCount;

        // Re-organized propagatorArray
        private BitArray[][] propagatorArrayDense;

        // Useful values
        private readonly WavePropagator propagator;
        private readonly int directionsCount;
        private readonly ITopology topology;
        private int indexCount;

        // List of locations that still need to be checked against for fulfilling the model's conditions
        private Stack<IndexPatternItem> toPropagate;

        /**
          * compatible[index, pattern, direction] contains the number of patterns present in the wave
          * that can be placed in the cell next to index in direction without being
          * in contradiction with pattern placed in index.
          * If possibilites[index][pattern] is set to false, then compatible[index, pattern, direction] has every direction negative or null
          */
        private int[,,] compatible;

        public Ac4PatternModelConstraint(WavePropagator propagator, PatternModel model)
        {
            this.toPropagate = new Stack<IndexPatternItem>();
            this.propagator = propagator;

            this.propagatorArray = model.Propagator;
            this.patternCount = model.PatternCount;

            this.propagatorArrayDense = model.Propagator.Select(a1 => a1.Select(x =>
            {
                var dense = new BitArray(this.patternCount);
                foreach (var p in x) dense[p] = true;
                return dense;
            }).ToArray()).ToArray();

            this.topology = propagator.Topology;
            this.indexCount = this.topology.IndexCount;
            this.directionsCount = this.topology.DirectionsCount;
        }

        public void Clear()
        {
            this.toPropagate.Clear();

            this.compatible = new int[this.indexCount, this.patternCount, this.directionsCount];

            var edgeLabels = new int[this.directionsCount];

            for (var index = 0; index < this.indexCount; index++)
            {
                if (!this.topology.ContainsIndex(index))
                    continue;

                // Cache edgeLabels
                for (var d = 0; d < this.directionsCount; d++)
                {
                    edgeLabels[d] = this.topology.TryMove(index, (Direction)d, out _, out var _, out var el) ? (int)el : -1;
                }

                for (var pattern = 0; pattern < this.patternCount; pattern++)
                {
                    for (var d = 0; d < this.directionsCount; d++)
                    {
                        var el = edgeLabels[d];
                        if (el >= 0)
                        {
                            var compatiblePatterns = this.propagatorArray[pattern][el].Length;
                            this.compatible[index, pattern, d] = compatiblePatterns;
                            if (compatiblePatterns == 0 && this.propagator.Wave.Get(index, pattern))
                            {
                                if (this.propagator.InternalBan(index, pattern))
                                {
                                    this.propagator.SetContradiction();
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        // Precondition that pattern at index is possible.
        public void DoBan(int index, int pattern)
        {
            // Update compatible (so that we never ban twice)
            for (var d = 0; d < this.directionsCount; d++)
            {
                this.compatible[index, pattern, d] -= this.patternCount;
            }
            // Queue any possible consequences of this changing.
            this.toPropagate.Push(new IndexPatternItem
            {
                Index = index,
                Pattern = pattern
            });
        }

        // This is equivalent to calling DoBan on every possible pattern
        // except the passed in one.
        // But it is more efficient.
        // Precondition that pattern at index is possible.
        public void DoSelect(int index, int pattern)
        {
            // Update compatible (so that we never ban twice)
            for (var p = 0; p < this.patternCount; p++)
            {
                if (p == pattern)
                    continue;
                for (var d = 0; d < this.directionsCount; d++)
                {
                    if (this.compatible[index, p, d] > 0)
                    {
                        this.compatible[index, p, d] -= this.patternCount;
                    }
                }
            }

            // Queue any possible consequences of this changing.
            this.toPropagate.Push(new IndexPatternItem
            {
                Index = index,
                Pattern = ~pattern
            });
        }

        public void UndoBan(int index, int pattern)
        {
            // Undo what was done in DoBan

            // First restore compatible for this cell
            // As it is set a negative value in InteralBan
            for (var d = 0; d < this.directionsCount; d++)
            {
                this.compatible[index, pattern, d] += this.patternCount;
            }

            // As we always Undo in reverse order, if item is in toPropagate, it'll
            // be at the top of the stack.
            // If item is in toPropagate, then we haven't got round to processing yet, so there's nothing to undo.
            if (this.toPropagate.Count > 0)
            {
                var top = this.toPropagate.Peek();
                if(top.Index == index && top.Pattern == pattern)
                {
                    this.toPropagate.Pop();
                    return;
                }
            }

            // Not in toPropagate, therefore undo what was done in Propagate
            for (var d = 0; d < this.directionsCount; d++)
            {
                if (!this.topology.TryMove(index, (Direction)d, out var i2, out var id, out var el))
                {
                    continue;
                }
                var patterns = this.propagatorArray[pattern][(int)el];
                foreach (var p in patterns)
                {
                    ++this.compatible[i2, p, (int)id];
                }
            }
        }

        private void PropagateBanCore(int[] patterns, int i2, int d)
        {
            // Hot loop
            foreach (var p in patterns)
            {
                var c = --this.compatible[i2, p, d];
                // Have we just now ruled out this possible pattern?
                if (c == 0)
                {
                    if (this.propagator.InternalBan(i2, p))
                    {
                        this.propagator.SetContradiction();
                    }
                }
            }
        }

        private void PropagateSelectCore(BitArray patternsDense, int i2, int id)
        {
            for (var p = 0; p < this.patternCount; p++)
            {
                var patternsContainsP = patternsDense[p];

                // Sets the value of compatible, triggering internal bans
                var prevCompatible = this.compatible[i2, p, (int)id];
                var currentlyPossible = prevCompatible > 0;
                var newCompatible = (currentlyPossible ? 0 : -this.patternCount) + (patternsContainsP ? 1 : 0);
                this.compatible[i2, p, (int)id] = newCompatible;

                // Have we just now ruled out this possible pattern?
                if (newCompatible == 0)
                {
                    if (this.propagator.InternalBan(i2, p))
                    {
                        this.propagator.SetContradiction();
                    }
                }
            }
        }

        public void Propagate()
        {
            while (this.toPropagate.Count > 0)
            {
                var item = this.toPropagate.Pop();
                int x, y, z;
                this.topology.GetCoord(item.Index, out x, out y, out z);
                if (item.Pattern >= 0)
                {
                    // Process a ban
                    for (var d = 0; d < this.directionsCount; d++)
                    {
                        if (!this.topology.TryMove(x, y, z, (Direction)d, out var i2, out var id, out EdgeLabel el))
                        {
                            continue;
                        }
                        var patterns = this.propagatorArray[item.Pattern][(int)el];
                        this.PropagateBanCore(patterns, i2, (int)id);
                    }
                }
                else
                {
                    // Process a select.
                    // Selects work similarly to bans, only instead of decrementing the compatible array
                    // we set it to a known value.
                    var pattern = ~item.Pattern;
                    for (var d = 0; d < this.directionsCount; d++)
                    {
                        if (!this.topology.TryMove(x, y, z, (Direction)d, out var i2, out var id, out EdgeLabel el))
                        {
                            continue;
                        }
                        var patternsDense = this.propagatorArrayDense[pattern][(int)el];

                        // TODO: Special case for when patterns.Length == 1?

                        this.PropagateSelectCore(patternsDense, i2, (int)id);


                    }
                }

                // It's important we fully process the item before returning
                // so that we're in a consistent state for backtracking
                // Hence we don't check this during the loops above
                if (this.propagator.Status == Resolution.Contradiction)
                {
                    return;
                }
            }
        }

    }
}
