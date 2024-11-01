using DeBroglie.Topo;
using DeBroglie.Trackers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeBroglie.Constraints
{
    internal class EdgedPathView : IPathView
    {
        ISet<Tile> endPointTiles;
        private IDictionary<Tile, ISet<Direction>> exits;
        private TilePropagatorTileSet pathTileSet;
        private SelectedTracker pathSelectedTracker;
        private TilePropagatorTileSet endPointTileSet;
        private SelectedTracker endPointSelectedTracker;

        private Dictionary<Direction, TilePropagatorTileSet> tileSetByExit;
        private Dictionary<Direction, SelectedTracker> trackerByExit;
        private bool hasEndPoints;

        private TilePropagator propagator;
        private ITopology topology;

        private List<int> endPointIndices;

        public EdgedPathView(EdgedPathSpec spec, TilePropagator propagator)
        {
            if (spec.TileRotation != null)
            {
                this.exits = new Dictionary<Tile, ISet<Direction>>();
                foreach (var kv in spec.Exits)
                {
                    foreach (var rot in spec.TileRotation.RotationGroup)
                    {
                        if (spec.TileRotation.Rotate(kv.Key, rot, out var rtile))
                        {
                            Direction Rotate(Direction d)
                            {
                                return TopoArrayUtils.RotateDirection(propagator.Topology.AsGridTopology().Directions, d, rot);
                            }
                            var rexits = new HashSet<Direction>(kv.Value.Select(Rotate));
                            this.exits[rtile] = rexits;
                        }
                    }
                }

                this.endPointTiles = spec.RelevantTiles == null ? null : new HashSet<Tile>(spec.TileRotation.RotateAll(spec.RelevantTiles));
            }
            else
            {
                this.exits = spec.Exits;
                this.endPointTiles = spec.RelevantTiles;
            }

            this.pathTileSet = propagator.CreateTileSet(this.exits.Keys);
            this.pathSelectedTracker = propagator.CreateSelectedTracker(this.pathTileSet);

            this.Graph = CreateEdgedGraph(propagator.Topology);
            this.propagator = propagator;
            this.topology = propagator.Topology;

            var nodesPerIndex = this.GetNodesPerIndex();

            this.CouldBePath = new bool[propagator.Topology.IndexCount * nodesPerIndex];
            this.MustBePath = new bool[propagator.Topology.IndexCount * nodesPerIndex];

            this.tileSetByExit = this.exits
                .SelectMany(kv => kv.Value.Select(e => Tuple.Create(kv.Key, e)))
                .GroupBy(x => x.Item2, x => x.Item1)
                .ToDictionary(g => g.Key, propagator.CreateTileSet);

            this.trackerByExit = this.tileSetByExit
                .ToDictionary(kv => kv.Key, kv => propagator.CreateSelectedTracker(kv.Value));

            this.hasEndPoints = spec.RelevantCells != null || spec.RelevantTiles != null;

            if(this.hasEndPoints)
            {
                this.CouldBeRelevant = new bool[propagator.Topology.IndexCount * nodesPerIndex];
                this.MustBeRelevant = new bool[propagator.Topology.IndexCount * nodesPerIndex];
                this.endPointIndices = spec.RelevantCells == null ? null :
                    spec.RelevantCells.Select(p => propagator.Topology.GetIndex(p.X, p.Y, p.Z)).ToList();
                this.endPointTileSet = this.endPointTiles != null ? propagator.CreateTileSet(this.endPointTiles) : null;
                this.endPointSelectedTracker = this.endPointTiles != null ? propagator.CreateSelectedTracker(this.endPointTileSet) : null;
            }
            else
            {
                this.CouldBeRelevant = this.CouldBePath;
                this.MustBeRelevant = this.MustBePath;
                this.endPointTileSet = this.pathTileSet;
            }
        }

        public SelectedTracker PathSelectedTracker => this.pathSelectedTracker;

        public Dictionary<Direction, SelectedTracker> TrackerByExit => this.trackerByExit;

        public Dictionary<Direction, TilePropagatorTileSet> TileSetByExit => this.tileSetByExit;


        public PathConstraintUtils.SimpleGraph Graph { get; }

        public bool[] CouldBePath { get; }
        public bool[] MustBePath { get; }
        public bool[] CouldBeRelevant { get; }
        public bool[] MustBeRelevant { get; }

        public void Update()
        {
            var nodesPerIndex = this.GetNodesPerIndex();
            var indexCount = this.topology.IndexCount;


            foreach (var kv in this.trackerByExit)
            {
                var exit = kv.Key;
                var tracker = kv.Value;
                for (var i = 0; i < indexCount; i++)
                {
                    var ts = tracker.GetQuadstate(i);
                    this.CouldBePath[i * nodesPerIndex + 1 + (int)exit] = ts.Possible();
                    this.MustBePath[i * nodesPerIndex + 1 + (int)exit] = ts.IsYes();
                }
            }
            for (var i = 0; i < indexCount; i++)
            {
                var pathTs = this.pathSelectedTracker.GetQuadstate(i);
                this.CouldBePath[i * nodesPerIndex] = pathTs.Possible();
                this.MustBePath[i * nodesPerIndex] = pathTs.IsYes();
            }
            if (this.hasEndPoints)
            {
                if (this.endPointIndices != null)
                {
                    foreach (var index in this.endPointIndices)
                    {
                        this.CouldBeRelevant[index * nodesPerIndex] = this.MustBeRelevant[index * nodesPerIndex] = true;
                    }
                }
                if (this.endPointSelectedTracker != null)
                {
                    for (var i = 0; i < indexCount; i++)
                    {
                        var ts = this.endPointSelectedTracker.GetQuadstate(i);
                        this.CouldBeRelevant[i * nodesPerIndex] = ts.Possible();
                        this.MustBeRelevant[i * nodesPerIndex] = ts.IsYes();
                    }
                }
            }
        }

        public void SelectPath(int node)
        {
            var (index, dir) = this.Unpack(node, this.GetNodesPerIndex());
            this.topology.GetCoord(index, out var x, out var y, out var z);
            if (dir == null)
            {
                this.propagator.Select(x, y, z, this.pathTileSet);
            }
            else
            {
                if (this.MustBePath[node])
                    return;
                if (this.tileSetByExit.TryGetValue((Direction)dir, out var exitTiles))
                {
                    this.propagator.Select(x, y, z, exitTiles);
                }
            }
        }

        public void BanPath(int node)
        {
            var (index, dir) = this.Unpack(node, this.GetNodesPerIndex());
            this.topology.GetCoord(index, out var x, out var y, out var z);
            if (dir == null)
            {
                this.propagator.Ban(x, y, z, this.pathTileSet);
            }
            else
            {
                if (this.tileSetByExit.TryGetValue((Direction)dir, out var exitTiles))
                {
                    this.propagator.Ban(x, y, z, exitTiles);
                }
            }
        }

        public void BanRelevant(int node)
        {
            var (index, dir) = this.Unpack(node, this.GetNodesPerIndex());
            this.topology.GetCoord(index, out var x, out var y, out var z);
            if (dir == null)
            {
                this.propagator.Ban(x, y, z, this.endPointTileSet);
            }
            else
            {
                // Ignore
            }
        }

        private int GetNodesPerIndex() => this.topology.DirectionsCount + 1;

        private (int, Direction?) Unpack(int node, int nodesPerIndex)
        {
            var index = node / nodesPerIndex;
            var d = node - (index * nodesPerIndex);
            if (d == 0)
                return (index, null);
            else
                return (index, (Direction?)(d - 1));
        }

        private int Pack(int index, Direction? dir, int nodesPerIndex)
        {
            return index * nodesPerIndex + (dir == null ? 0 : 1 + (int)dir);
        }

        private static readonly int[] Empty = { };

        /// <summary>
        /// Creates a grpah where each index in the original topology
        /// has 1+n nodes in the graph - one for the initial index
        /// and one for each direction leading out of it.
        /// </summary>
        private static PathConstraintUtils.SimpleGraph CreateEdgedGraph(ITopology topology)
        {
            var nodesPerIndex = topology.DirectionsCount + 1;

            var nodeCount = topology.IndexCount * nodesPerIndex;

            var neighbours = new int[nodeCount][];

            int GetNodeId(int index) => index * nodesPerIndex;

            int GetDirNodeId(int index, Direction direction) => index * nodesPerIndex + 1 + (int)direction;

            foreach (var i in topology.GetIndices())
            {
                var n = new List<int>();
                for (var d = 0; d < topology.DirectionsCount; d++)
                {
                    var direction = (Direction)d;
                    // The central node connects to the direction node
                    n.Add(GetDirNodeId(i, direction));
                    if (topology.TryMove(i, direction, out var dest, out var inverseDir, out var _))
                    {
                        // The diction node connects to the central node
                        // and the opposing direction node
                        neighbours[GetDirNodeId(i, direction)] =
                            new[] { GetNodeId(i), GetDirNodeId(dest, inverseDir) };
                    }
                    else
                    {
                        // Dead end
                        neighbours[GetDirNodeId(i, direction)] =
                            new[] { GetNodeId(i)};
                    }
                }
                neighbours[GetNodeId(i)] = n.ToArray();
            }

            return new PathConstraintUtils.SimpleGraph
            {
                NodeCount = nodeCount,
                Neighbours = neighbours
            };
        }
    }
}
