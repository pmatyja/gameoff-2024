using System;
using System.Collections.Generic;
using DeBroglie.Topo;
using DeBroglie.Trackers;

namespace DeBroglie.Wfc
{

    internal class WavePropagatorOptions
    {
        public IBacktrackPolicy BacktrackPolicy { get; set; }
        public int MaxBacktrackDepth { get; set; }
        public IWaveConstraint[] Constraints { get; set; }
        public Func<double> RandomDouble { get; set; }
        public IIndexPicker IndexPicker { get; set; }
        public IPatternPicker PatternPicker { get; set; }
        public bool Clear { get; set; } = true;
        public ModelConstraintAlgorithm ModelConstraintAlgorithm { get; set; }
    }

    /// <summary>
    /// WavePropagator holds a wave, and supports updating it's possibilities 
    /// according to the model constraints.
    /// </summary>
    internal class WavePropagator
    {
        // Main data tracking what we've decided so far
        private Wave wave;

        private IPatternModelConstraint patternModelConstraint;

        // From model
        private int patternCount;
        private double[] frequencies;

        // Used for backtracking
        private Deque<IndexPatternItem> backtrackItems;
        private Deque<int> backtrackItemsLengths;
        private Deque<IndexPatternItem> prevChoices;
        // Used for MaxBacktrackDepth
        private int droppedBacktrackItemsCount;
        // In
        private int backtrackCount; // Purely informational
        private int backjumpCount; // Purely informational

        // Basic parameters
        private int indexCount;
        private readonly bool backtrack;
        private readonly int maxBacktrackDepth;
        private readonly IWaveConstraint[] constraints;
        private Func<double> randomDouble;

        // We evaluate constraints at the last possible minute, instead of eagerly like the model,
        // As they can potentially be expensive.
        private bool deferredConstraintsStep;

        // The overall status of the propagator, always kept up to date
        private Resolution status;

        public string contradictionReason;
        public object contradictionSource;

        private ITopology topology;
        private int directionsCount;

        private List<ITracker> trackers;
        private List<IChoiceObserver> choiceObservers;

        private readonly IIndexPicker indexPicker;
        private readonly IPatternPicker patternPicker;
        private IBacktrackPolicy backtrackPolicy;

        public WavePropagator(
            PatternModel model,
            ITopology topology,
            WavePropagatorOptions options)
        {
            this.patternCount = model.PatternCount;
            this.frequencies = model.Frequencies;

            this.indexCount = topology.IndexCount;
            this.backtrack = options.BacktrackPolicy != null;
            this.backtrackPolicy = options.BacktrackPolicy;
            this.maxBacktrackDepth = options.MaxBacktrackDepth;
            this.constraints = options.Constraints ?? new IWaveConstraint[0];
            this.topology = topology;
            this.randomDouble = options.RandomDouble ?? new Random().NextDouble;
            this.directionsCount = topology.DirectionsCount;
            this.indexPicker = options.IndexPicker ?? new EntropyTracker();
            this.patternPicker = options.PatternPicker ?? new WeightedRandomPatternPicker();

            switch (options.ModelConstraintAlgorithm)
            {
                case ModelConstraintAlgorithm.OneStep:
                    this.patternModelConstraint = new OneStepPatternModelConstraint(this, model);
                    break;
                case ModelConstraintAlgorithm.Default:
                case ModelConstraintAlgorithm.Ac4:
                    this.patternModelConstraint = new Ac4PatternModelConstraint(this, model);
                    break;
                case ModelConstraintAlgorithm.Ac3:
                    this.patternModelConstraint = new Ac3PatternModelConstraint(this, model);
                    break;
                default:
                    throw new Exception();
            }

            if (options.Clear) this.Clear();
        }

        // This is only exposed publically
        // in case users write their own constraints, it's not 
        // otherwise useful.
        #region Internal API

        public Wave Wave => this.wave;
        public int IndexCount => this.indexCount;
        public ITopology Topology => this.topology;
        public Func<double> RandomDouble => this.randomDouble;

        public int PatternCount => this.patternCount;
        public double[] Frequencies => this.frequencies;

        /**
         * Requires that index, pattern is possible
         */
        public bool InternalBan(int index, int pattern)
        {
            // Record information for backtracking
            if (this.backtrack)
            {
                this.backtrackItems.Push(new IndexPatternItem
                {
                    Index = index,
                    Pattern = pattern
                });
            }

            this.patternModelConstraint.DoBan(index, pattern);
            
            // Update the wave
            var isContradiction = this.wave.RemovePossibility(index, pattern);

            // Update trackers
            foreach (var tracker in this.trackers)
            {
                tracker.DoBan(index, pattern);
            }

            return isContradiction;
        }

        public bool InternalSelect(int index, int chosenPattern)
        {
            // Simple, inefficient way
            if (!Optimizations.QuickSelect)
            {
                for (var pattern = 0; pattern < this.patternCount; pattern++)
                {
                    if (pattern == chosenPattern)
                    {
                        continue;
                    }
                    if (this.wave.Get(index, pattern))
                    {
                        if (this.InternalBan(index, pattern))
                            return true;
                    }
                }
                return false;
            }

            #if _UNUSED
            var isContradiction = false;

            this.patternModelConstraint.DoSelect(index, chosenPattern);

            for (var pattern = 0; pattern < this.patternCount; pattern++)
            {
                if (pattern == chosenPattern)
                {
                    continue;
                }
                if (this.wave.Get(index, pattern))
                {
                    // This is mostly a repeat of InternalBan, as except for patternModelConstraint,
                    // Selects are just seen as a set of bans


                    // Record information for backtracking
                    if (this.backtrack)
                    {
                        this.backtrackItems.Push(new IndexPatternItem
                        {
                            Index = index,
                            Pattern = pattern
                        });
                    }

                    // Don't update patternModelConstraint here, it's been done above in DoSelect

                    // Update the wave
                    isContradiction = isContradiction || this.wave.RemovePossibility(index, pattern);

                    // Update trackers
                    foreach (var tracker in this.trackers)
                    {
                        tracker.DoBan(index, pattern);
                    }

                }
            }
            return false;
            #endif
        }
        #endregion

        /// <summary>
        /// Returns the only possible value of a cell if there is only one, 
        /// otherwise returns -1 (multiple possible) or -2 (none possible)
        /// </summary>
        public int GetDecidedPattern(int index)
        {
            var decidedPattern = (int)Resolution.Contradiction;
            for (var pattern = 0; pattern < this.patternCount; pattern++)
            {
                if (this.wave.Get(index, pattern))
                {
                    if (decidedPattern == (int)Resolution.Contradiction)
                    {
                        decidedPattern = pattern;
                    }
                    else
                    {
                        return (int)Resolution.Undecided;
                    }
                }
            }
            return decidedPattern;
        }

        private void InitConstraints()
        {
            foreach (var constraint in this.constraints)
            {
                constraint.Init(this);
                if (this.status != Resolution.Undecided) return;
                this.patternModelConstraint.Propagate();
                if (this.status != Resolution.Undecided) return;
            }
            return;
        }

        public void StepConstraints()
        {
            // TODO: Do we need to worry about evaluating constraints multiple times?
            foreach (var constraint in this.constraints)
            {
                constraint.Check(this);
                if (this.status != Resolution.Undecided) return;
                this.patternModelConstraint.Propagate();
                if (this.status != Resolution.Undecided) return;
            }

            this.deferredConstraintsStep = false;
        }

        public Resolution Status => this.status;
        public string ContradictionReason => this.contradictionReason;
        public object ContradictionSource => this.contradictionSource;
        public int BacktrackCount => this.backtrackCount;
        public int BackjumpCount => this.backjumpCount;

        /**
         * Resets the wave to it's original state
         */
        public Resolution Clear()
        {
            this.wave = new Wave(this.frequencies.Length, this.indexCount);

            if (this.backtrack)
            {
                this.backtrackItems = new Deque<IndexPatternItem>();
                this.backtrackItemsLengths = new Deque<int>();
                this.backtrackItemsLengths.Push(0);
                this.prevChoices = new Deque<IndexPatternItem>();
            }

            this.status = Resolution.Undecided;
            this.contradictionReason = null;
            this.contradictionSource = null;
            this.trackers = new List<ITracker>();
            this.choiceObservers = new List<IChoiceObserver>();
            this.indexPicker.Init(this);
            this.patternPicker.Init(this);
            this.backtrackPolicy?.Init(this);

            this.patternModelConstraint.Clear();

            if (this.status == Resolution.Contradiction) return this.status;

            this.InitConstraints();

            return this.status;
        }

        /**
         * Indicates that the generation cannot proceed, forcing the algorithm to backtrack or exit.
         */
        public void SetContradiction()
        {
            this.status = Resolution.Contradiction;
        }

        /**
         * Indicates that the generation cannot proceed, forcing the algorithm to backtrack or exit.
         */
        public void SetContradiction(string reason, object source)
        {
            this.status = Resolution.Contradiction;
            this.contradictionReason = reason;
            this.contradictionSource = source;
        }

        /**
         * Removes pattern as a possibility from index
         */
        public Resolution Ban(int x, int y, int z, int pattern)
        {
            var index = this.topology.GetIndex(x, y, z);
            if (this.wave.Get(index, pattern))
            {
                this.deferredConstraintsStep = true;
                if (this.InternalBan(index, pattern))
                {
                    return this.status = Resolution.Contradiction;
                }
            }

            this.patternModelConstraint.Propagate();
            return this.status;
        }

        /**
         * Make some progress in the WaveFunctionCollapseAlgorithm
         */
        public Resolution Step()
        {
            // This will be true if the user has called Ban, etc, since the last step.
            if (this.deferredConstraintsStep)
            {
                this.StepConstraints();
            }

            // If we're already in a final state. skip making an observiation.
            if (this.status == Resolution.Undecided)
            {
                // Pick a index to use
                var index = this.indexPicker.GetRandomIndex(this.randomDouble);

                if (index != -1)
                {
                    // Pick a tile to select at that index
                    var pattern = this.patternPicker.GetRandomPossiblePatternAt(index, this.randomDouble);

                    this.RecordBacktrack(index, pattern);

                    // Use the pick
                    if (this.InternalSelect(index, pattern))
                    {
                        this.status = Resolution.Contradiction;
                    }
                }

                // Re-evaluate status
                if (this.status == Resolution.Undecided) this.patternModelConstraint.Propagate();
                if (this.status == Resolution.Undecided) this.StepConstraints();

                // If we've made all possible choices, and found no contradictions,
                // then we've succeeded.
                if (index == -1 && this.status == Resolution.Undecided)
                {
                    this.status = Resolution.Decided;
                    return this.status;
                }
            }

            this.TryBacktrackUntilNoContradiction();

            return this.status;
        }

        private void RecordBacktrack(int index, int pattern)
        {
            if (!this.backtrack)
                return;

            this.backtrackItemsLengths.Push(this.droppedBacktrackItemsCount + this.backtrackItems.Count);
            this.prevChoices.Push(new IndexPatternItem { Index = index, Pattern = pattern });

            foreach (var co in this.choiceObservers)
            {
                co.MakeChoice(index, pattern);
            }

            // Clean up backtracks if they are too long
            while (this.maxBacktrackDepth > 0 && this.backtrackItemsLengths.Count > this.maxBacktrackDepth)
            {
                var newDroppedCount = this.backtrackItemsLengths.Unshift();
                this.prevChoices.Unshift();
                this.backtrackItems.DropFirst(newDroppedCount - this.droppedBacktrackItemsCount);
                this.droppedBacktrackItemsCount = newDroppedCount;
            }

        }

        private void TryBacktrackUntilNoContradiction()
        {
            if (!this.backtrack)
                return;

            while (this.status == Resolution.Contradiction)
            {
                var backjumpAmount = this.backtrackPolicy.GetBackjump();

                for (var i = 0; i < backjumpAmount; i++)
                {
                    if (this.backtrackItemsLengths.Count == 1)
                    {
                        // We've backtracked as much as we can, but 
                        // it's still not possible. That means it is imposible
                        return;
                    }

                    // Actually undo various bits of state
                    this.DoBacktrack();
                    var item = this.prevChoices.Pop();
                    this.status = Resolution.Undecided;
                    this.contradictionReason = null;
                    this.contradictionSource = null;
                    foreach (var co in this.choiceObservers)
                    {
                        co.Backtrack();
                    }

                    if (backjumpAmount == 1)
                    {
                        this.backtrackCount++;

                        // Mark the given choice as impossible
                        if (this.InternalBan(item.Index, item.Pattern))
                        {
                            this.status = Resolution.Contradiction;
                        }
                    }
                }

                if(backjumpAmount > 1)
                {
                    this.backjumpCount++;
                }

                // Revalidate status.
                if (this.status == Resolution.Undecided) this.patternModelConstraint.Propagate();
                if (this.status == Resolution.Undecided) this.StepConstraints();
            }
        }

        // Actually does the work of undoing what was previously recorded
        private void DoBacktrack()
        {
            var targetLength = this.backtrackItemsLengths.Pop() - this.droppedBacktrackItemsCount;
            // Undo each item
            while (this.backtrackItems.Count > targetLength)
            {
                var item = this.backtrackItems.Pop();
                var index = item.Index;
                var pattern = item.Pattern;

                // Also add the possibility back
                // as it is removed in InternalBan
                this.wave.AddPossibility(index, pattern);
                // Update trackers
                foreach(var tracker in this.trackers)
                {
                    tracker.UndoBan(index, pattern);
                }
                // Next, undo the decremenents done in Propagate
                this.patternModelConstraint.UndoBan(index, pattern);

            }
        }

        public void AddTracker(ITracker tracker)
        {
            this.trackers.Add(tracker);
        }

        public void RemoveTracker(ITracker tracker)
        {
            this.trackers.Remove(tracker);
        }

        public void AddChoiceObserver(IChoiceObserver co)
        {
            this.choiceObservers.Add(co);
        }

        public void RemoveChoiceObserver(IChoiceObserver co)
        {
            this.choiceObservers.Remove(co);
        }

        /**
         * Rpeatedly step until the status is Decided or Contradiction
         */
        public Resolution Run()
        {
            while (true)
            {
                this.Step();
                if (this.status != Resolution.Undecided) return this.status;
            }
        }

        /**
         * Returns the array of decided patterns, writing
         * -1 or -2 to indicate cells that are undecided or in contradiction.
         */
        public ITopoArray<int> ToTopoArray()
        {
            return TopoArray.CreateByIndex(this.GetDecidedPattern, this.topology);
        }

        /**
         * Returns an array where each cell is a list of remaining possible patterns.
         */
        public ITopoArray<ISet<int>> ToTopoArraySets()
        {
            return TopoArray.CreateByIndex(index =>
            {
                var hs = new HashSet<int>();
                for (var pattern = 0; pattern < this.patternCount; pattern++)
                {
                    if (this.wave.Get(index, pattern))
                    {
                        hs.Add(pattern);
                    }
                }

                return (ISet<int>)(hs);
            }, this.topology);
        }

        public IEnumerable<int> GetPossiblePatterns(int index)
        {
            for (var pattern = 0; pattern < this.patternCount; pattern++)
            {
                if (this.wave.Get(index, pattern))
                {
                    yield return pattern;
                }
            }
        }
    }
}
