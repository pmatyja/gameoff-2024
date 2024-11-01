using DeBroglie.Topo;
using DeBroglie.Trackers;
using System.Collections.Generic;

namespace DeBroglie.Constraints
{
    /// <summary>
    /// This constraint forces one set of tiles to not be placed near another set.
    /// </summary>
    public class PairSeparationConstraint : ITileConstraint
    {
        private TilePropagatorTileSet tileset1;
        private TilePropagatorTileSet tileset2;
        private SelectedChangeTracker changeTracker1;
        private SelectedChangeTracker changeTracker2;
        private SeparationConstraint.NearbyTracker nearbyTracker1;
        private SeparationConstraint.NearbyTracker nearbyTracker2;

        public ISet<Tile> Tiles1 { get; set; }
        public ISet<Tile> Tiles2 { get; set; }

        /// <summary>
        /// The minimum distance between two points.
        /// Measured using manhattan distance.
        /// </summary>
        public int MinDistance { get; set; }


        public void Init(TilePropagator propagator)
        {
            this.tileset1 = propagator.CreateTileSet(this.Tiles1);
            this.tileset2 = propagator.CreateTileSet(this.Tiles2);
            this.nearbyTracker1 = new SeparationConstraint.NearbyTracker { MinDistance = this.MinDistance, Topology = propagator.Topology };
            this.nearbyTracker2 = new SeparationConstraint.NearbyTracker { MinDistance = this.MinDistance, Topology = propagator.Topology };
            this.changeTracker1 = propagator.CreateSelectedChangeTracker(this.tileset1, this.nearbyTracker1);
            this.changeTracker2 = propagator.CreateSelectedChangeTracker(this.tileset2, this.nearbyTracker2);

            // Review the initial state
            foreach(var index in propagator.Topology.GetIndices())
            {
                if (this.changeTracker1.GetQuadstate(index).IsYes())
                {
                    this.nearbyTracker1.VisitNearby(index, false);
                }
                if (this.changeTracker2.GetQuadstate(index).IsYes())
                {
                    this.nearbyTracker2.VisitNearby(index, false);
                }
            }

            this.Check(propagator);
        }

        public void Check(TilePropagator propagator)
        {
            if (this.nearbyTracker1.NewlyVisited.Count != 0)
            {
                var newlyVisited = this.nearbyTracker1.NewlyVisited;
                this.nearbyTracker1.NewlyVisited = new HashSet<int>();

                foreach (var index in newlyVisited)
                {
                    propagator.Topology.GetCoord(index, out var x, out var y, out var z);
                    propagator.Ban(x, y, z, this.tileset2);
                }
            }
            if (this.nearbyTracker2.NewlyVisited.Count != 0)
            {
                var newlyVisited = this.nearbyTracker2.NewlyVisited;
                this.nearbyTracker2.NewlyVisited = new HashSet<int>();

                foreach (var index in newlyVisited)
                {
                    propagator.Topology.GetCoord(index, out var x, out var y, out var z);
                    propagator.Ban(x, y, z, this.tileset1);
                }
            }
        }
    }
}
