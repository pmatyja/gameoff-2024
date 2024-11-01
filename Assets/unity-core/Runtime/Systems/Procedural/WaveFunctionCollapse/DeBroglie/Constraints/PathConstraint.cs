using DeBroglie.Rot;
using DeBroglie.Trackers;
using System;
using System.Collections.Generic;

namespace DeBroglie.Constraints
{
    /// <summary>
    /// The PathConstraint checks that it is possible to connect several locations together via a continuous path of adjacent tiles. 
    /// It does this by banning any tile placement that would make such a path impossible.
    /// </summary>
    [Obsolete("Use ConnectedConstraint instead")]
    public class PathConstraint : ITileConstraint
    {
        private TilePropagatorTileSet tileSet;

        private SelectedTracker selectedTracker;

        private TilePropagatorTileSet endPointTileSet;

        private SelectedTracker endPointSelectedTracker;

        private PathConstraintUtils.SimpleGraph graph;

        /// <summary>
        /// Set of patterns that are considered on the path
        /// </summary>
        public ISet<Tile> Tiles { get; set; }

        /// <summary>
        /// Set of points that must be connected by paths.
        /// If EndPoints and EndPointTiles are null, then PathConstraint ensures that all path cells
        /// are connected.
        /// </summary>
        public Point[] EndPoints { get; set; }

        /// <summary>
        /// Set of tiles that must be connected by paths.
        /// If EndPoints and EndPointTiles are null, then PathConstraint ensures that all path cells
        /// are connected.
        /// </summary>
        public ISet<Tile> EndPointTiles { get; set; }

        /// <summary>
        /// If set, Tiles is augmented with extra copies as dictated by the tile rotations
        /// </summary>
        public TileRotation TileRotation { get; set; }


        public PathConstraint(ISet<Tile> tiles, Point[] endPoints = null, TileRotation tileRotation = null)
        {
            this.Tiles = tiles;
            this.EndPoints = endPoints;
            this.TileRotation = tileRotation;
        }

        public void Init(TilePropagator propagator)
        {
            ISet<Tile> actualTiles;
            ISet<Tile> actualEndPointTiles;
            if (this.TileRotation != null)
            {
                actualTiles = new HashSet<Tile>(this.TileRotation.RotateAll(this.Tiles));
                actualEndPointTiles = this.EndPointTiles == null ? null : new HashSet<Tile>(this.TileRotation.RotateAll(this.EndPointTiles));
            }
            else
            {
                actualTiles = this.Tiles;
                actualEndPointTiles = this.EndPointTiles;
            }

            this.tileSet = propagator.CreateTileSet(actualTiles);
            this.selectedTracker = propagator.CreateSelectedTracker(this.tileSet);
            this.endPointTileSet = this.EndPointTiles != null ? propagator.CreateTileSet(actualEndPointTiles) : null;
            this.endPointSelectedTracker = this.EndPointTiles != null ? propagator.CreateSelectedTracker(this.endPointTileSet) : null;
            this.graph = PathConstraintUtils.CreateGraph(propagator.Topology);

            this.Check(propagator, true);
        }

        public void Check(TilePropagator propagator)
        {
            this.Check(propagator, false);
        }

        private void Check(TilePropagator propagator, bool init)
        {
            var topology = propagator.Topology;
            var indices = topology.IndexCount;
            // Initialize couldBePath and mustBePath based on wave possibilities
            var couldBePath = new bool[indices];
            var mustBePath = new bool[indices];
            for (var i = 0; i < indices; i++)
            {
                var ts = this.selectedTracker.GetQuadstate(i);
                couldBePath[i] = ts.Possible();
                mustBePath[i] = ts.IsYes();
            }

            // Select relevant cells, i.e. those that must be connected.
            var hasEndPoints = this.EndPoints != null || this.EndPointTiles != null;
            bool[] relevant;
            if (!hasEndPoints)
            {
                relevant = mustBePath;
            }
            else
            {
                relevant = new bool[indices];
                var relevantCount = 0;
                if (this.EndPoints != null)
                {
                    foreach (var endPoint in this.EndPoints)
                    {
                        var index = topology.GetIndex(endPoint.X, endPoint.Y, endPoint.Z);
                        relevant[index] = true;
                        relevantCount++;
                    }
                }
                if (this.EndPointTiles != null)
                {
                    for (var i = 0; i < indices; i++)
                    {
                        if (this.endPointSelectedTracker.IsSelected(i))
                        {
                            relevant[i] = true;
                            relevantCount++;
                        }
                    }
                }
                if (relevantCount == 0)
                {
                    // Nothing to do.
                    return;
                }
            }

            if(init)
            {
                for (var i = 0; i < indices; i++)
                {
                    if(relevant[i])
                    {
                        topology.GetCoord(i, out var x, out var y, out var z);
                        propagator.Select(x, y, z, this.tileSet);
                    }
                }
            }

            var walkable = couldBePath;

            var info = PathConstraintUtils.GetArticulationPoints(this.graph, walkable, relevant);
            var isArticulation = info.IsArticulation;

            if (info.ComponentCount > 1)
            {
                propagator.SetContradiction("Path constraint found multiple components", this);
                return;
            }

            // All articulation points must be paths,
            // So ban any other possibilities
            for (var i = 0; i < indices; i++)
            {
                if (isArticulation[i] && !mustBePath[i])
                {
                    topology.GetCoord(i, out var x, out var y, out var z);
                    propagator.Select(x, y, z, this.tileSet);
                }
            }

            // Any path tiles / EndPointTiles not in the connected component aren't safe to add.
            if (info.ComponentCount > 0)
            {
                var component = info.Component;
                var actualEndPointTileSet = hasEndPoints ? this.endPointTileSet : this.tileSet;
                if (actualEndPointTileSet != null)
                {
                    for (var i = 0; i < indices; i++)
                    {
                        if (component[i] == null)
                        {
                            topology.GetCoord(i, out var x, out var y, out var z);
                            propagator.Ban(x, y, z, actualEndPointTileSet);
                        }
                    }
                }
            }
        }
    }
}
