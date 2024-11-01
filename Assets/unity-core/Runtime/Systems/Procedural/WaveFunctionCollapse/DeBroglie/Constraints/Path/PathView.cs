using DeBroglie.Trackers;
using System.Collections.Generic;
using System.Linq;

namespace DeBroglie.Constraints
{
    internal class PathView : IPathView
    {
        private readonly ISet<Tile> tiles;
        private readonly ISet<Tile> endPointTiles;
        private readonly TilePropagatorTileSet tileSet;
        private readonly SelectedTracker selectedTracker;

        private bool hasEndPoints;
        private readonly List<int> endPointIndices;
        private readonly TilePropagatorTileSet endPointTileSet;
        private readonly SelectedTracker endPointSelectedTracker;

        private readonly TilePropagator propagator;

        public PathView(PathSpec spec, TilePropagator propagator)
        {

            if (spec.TileRotation != null)
            {
                this.tiles = new HashSet<Tile>(spec.TileRotation.RotateAll(spec.Tiles));
                this.endPointTiles = spec.RelevantTiles == null ? null : new HashSet<Tile>(spec.TileRotation.RotateAll(spec.RelevantTiles));
            }
            else
            {
                this.tiles = spec.Tiles;
                this.endPointTiles = spec.RelevantTiles;
            }

            this.tileSet = propagator.CreateTileSet(this.tiles);
            this.selectedTracker = propagator.CreateSelectedTracker(this.tileSet);

            this.Graph = PathConstraintUtils.CreateGraph(propagator.Topology);
            this.propagator = propagator;

            this.CouldBePath = new bool[propagator.Topology.IndexCount];
            this.MustBePath = new bool[propagator.Topology.IndexCount];

            this.hasEndPoints = spec.RelevantCells != null || spec.RelevantTiles != null;

            if (this.hasEndPoints)
            {
                this.CouldBeRelevant = new bool[propagator.Topology.IndexCount];
                this.MustBeRelevant = new bool[propagator.Topology.IndexCount];
                this.endPointIndices = spec.RelevantCells == null ? null :
                    spec.RelevantCells.Select(p => propagator.Topology.GetIndex(p.X, p.Y, p.Z)).ToList();
                this.endPointTileSet = spec.RelevantTiles != null ? propagator.CreateTileSet(this.endPointTiles) : null;
                this.endPointSelectedTracker = spec.RelevantTiles != null ? propagator.CreateSelectedTracker(this.endPointTileSet) : null;
            }
            else
            {
                this.CouldBeRelevant = this.CouldBePath;
                this.MustBeRelevant = this.MustBePath;
                this.endPointTileSet = this.tileSet;
            }

        }

        public PathConstraintUtils.SimpleGraph Graph { get; }

        public bool[] CouldBePath { get; }
        public bool[] MustBePath { get; }


        public bool[] CouldBeRelevant { get; }
        public bool[] MustBeRelevant { get; }


        public void Update()
        {
            var topology = this.propagator.Topology;
            var indexCount = topology.IndexCount;
            for (var i = 0; i < indexCount; i++)
            {
                var ts = this.selectedTracker.GetQuadstate(i);
                this.CouldBePath[i] = ts.Possible();
                this.MustBePath[i] = ts.IsYes();
            }

            if (this.hasEndPoints)
            {
                if (this.endPointIndices != null)
                {
                    foreach (var index in this.endPointIndices)
                    {
                        this.CouldBeRelevant[index] = this.MustBeRelevant[index] = true;
                    }
                }
                if (this.endPointSelectedTracker != null)
                {
                    for (var i = 0; i < indexCount; i++)
                    {
                        var ts = this.endPointSelectedTracker.GetQuadstate(i);

                        this.CouldBeRelevant[i] = ts.Possible();
                        this.MustBeRelevant[i] = ts.IsYes();
                    }
                }
            }
        }

        public void SelectPath(int index)
        {
            this.propagator.Topology.GetCoord(index, out var x, out var y, out var z);
            this.propagator.Select(x, y, z, this.tileSet);
        }

        public void BanPath(int index)
        {
            this.propagator.Topology.GetCoord(index, out var x, out var y, out var z);
            this.propagator.Ban(x, y, z, this.tileSet);
        }

        public void BanRelevant(int index)
        {
            this.propagator.Topology.GetCoord(index, out var x, out var y, out var z);
            this.propagator.Ban(x, y, z, this.endPointTileSet);
        }
    }
}
