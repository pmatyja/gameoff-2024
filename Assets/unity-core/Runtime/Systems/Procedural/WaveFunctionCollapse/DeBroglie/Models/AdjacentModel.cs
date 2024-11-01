using DeBroglie.Rot;
using DeBroglie.Topo;
using DeBroglie.Wfc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeBroglie.Models
{


    /// <summary>
    /// AdjacentModel constrains which tiles can be placed adjacent to which other ones. 
    /// It does so by maintaining for each tile, a list of tiles that can be placed next to it in each direction. 
    /// The list is always symmetric, i.e. if it is legal to place tile B directly above tile A, then it is legal to place A directly below B.
    /// </summary>
    public class AdjacentModel : TileModel
    {
        private DirectionSet directions;
        private Dictionary<Tile, int> tilesToPatterns;
        private List<double> frequencies;
        private List<HashSet<int>[]> propagator;

        /// <summary>
        /// Constructs an AdjacentModel and initializees it with a given sample.
        /// </summary>
        public static AdjacentModel Create<T>(T[,] sample, bool periodic)
        {
            return Create(new TopoArray2D<T>(sample, periodic));
        }

        /// <summary>
        /// Constructs an AdjacentModel and initializees it with a given sample.
        /// </summary>
        public static AdjacentModel Create<T>(ITopoArray<T> sample)
        {
            return new AdjacentModel(sample.ToTiles());
        }


        /// <summary>
        /// Constructs an AdjacentModel.
        /// </summary>
        public AdjacentModel()
        {
            // Tiles map 1:1 with patterns
            this.tilesToPatterns = new Dictionary<Tile, int>();
            this.frequencies = new List<double>();
            this.propagator = new List<HashSet<int>[]>();
        }

        /// <summary>
        /// Constructs an AdjacentModel.
        /// </summary>
        public AdjacentModel(DirectionSet directions)
            : this()
        {
            this.SetDirections(directions);
        }



        /// <summary>
        /// Constructs an AdjacentModel and initializees it with a given sample.
        /// </summary>
        public AdjacentModel(ITopoArray<Tile> sample)
            : this()
        {
            this.AddSample(sample);
        }

        public override IEnumerable<Tile> Tiles => this.tilesToPatterns.Keys;

        /// <summary>
        /// Sets the directions of the Adjacent model, if it has not been set at construction.
        /// This specifies how many neighbours each tile has.
        /// Once set, it cannot be changed.
        /// </summary>
        /// <param name="directions"></param>
        public void SetDirections(DirectionSet directions)
        {
            if (this.directions.Type != DirectionSetType.Unknown && this.directions.Type != directions.Type)
            {
                throw new Exception($"Cannot set directions to {directions.Type}, it has already been set to {this.directions.Type}");
            }

            this.directions = directions;
        }

        private void RequireDirections()
        {
            if(this.directions.Type == DirectionSetType.Unknown)
            {
                throw new Exception("Directions must be set before calling this method");
            }
        }

        /// <summary>
        /// Finds a tile and all its rotations, and sets their total frequency.
        /// </summary>
        public void SetFrequency(Tile tile, double frequency, TileRotation tileRotation)
        {
            var rotatedTiles = tileRotation.RotateAll(tile).ToList();
            foreach (var rt in rotatedTiles)
            {
                var pattern = this.GetPattern(rt);
                this.frequencies[pattern] = 0.0;
            }
            var incrementalFrequency = frequency / rotatedTiles.Count;
            foreach (var rt in rotatedTiles)
            {
                var pattern = this.GetPattern(rt);
                this.frequencies[pattern] += incrementalFrequency;
            }
        }


        /// <summary>
        /// Sets the frequency of a given tile.
        /// </summary>
        public void SetFrequency(Tile tile, double frequency)
        {
            var pattern = this.GetPattern(tile);
            this.frequencies[pattern] = frequency;
        }

        /// <summary>
        /// Sets all tiles as equally likely to be picked
        /// </summary>
        public void SetUniformFrequency()
        {
            foreach(var tile in this.Tiles)
            {
                this.SetFrequency(tile, 1.0);
            }
        }

        /// <summary>
        /// Declares that the tiles in dest can be placed adjacent to the tiles in src, in the direction specified.
        /// Then it adds similar declarations for other rotations and reflections, as specified by rotations.
        /// </summary>
        public void AddAdjacency(IList<Tile> src, IList<Tile> dest, Direction dir, TileRotation tileRotation = null)
        {
            this.RequireDirections();
            var d = (int)dir;
            var x = this.directions.DX[d];
            var y = this.directions.DY[d];
            var z = this.directions.DZ[d];
            this.AddAdjacency(src, dest, x, y, z, tileRotation);
        }

        /// <summary>
        /// Declares that the tiles in dest can be placed adjacent to the tiles in src, in the direction specified by (x, y, z).
        /// Then it adds similar declarations for other rotations and reflections, as specified by rotations.
        /// </summary>
        public void AddAdjacency(IList<Tile> src, IList<Tile> dest, int x, int y, int z, TileRotation tileRotation = null)
        {
            this.RequireDirections();

            tileRotation ??= new TileRotation();

            foreach (var rotation in tileRotation.RotationGroup)
            {
                var (x2, y2) = TopoArrayUtils.RotateVector(this.directions.Type, x, y, rotation);

                this.AddAdjacency(
                    tileRotation.Rotate(src, rotation).ToList(),
                    tileRotation.Rotate(dest, rotation).ToList(),
                    x2, y2, z);
            }
        }

        /// <summary>
        /// Declares that the tiles in dest can be placed adjacent to the tiles in src, in the direction specified by (x, y, z).
        /// (x, y, z) must be a valid direction, which usually means a unit vector.
        /// </summary>
        public void AddAdjacency(IList<Tile> src, IList<Tile> dest, int x, int y, int z)
        {
            this.RequireDirections();
            this.AddAdjacency(src, dest, this.directions.GetDirection(x, y, z));
        }

        /// <summary>
        /// Declares that the tiles in dest can be placed adjacent to the tiles in src, in the direction specified by (x, y, z).
        /// (x, y, z) must be a valid direction, which usually means a unit vector.
        /// </summary>
        public void AddAdjacency(IList<Tile> src, IList<Tile> dest, Direction dir)
        {
            this.RequireDirections();

            foreach (var s in src)
            {
                foreach (var d in dest)
                {
                    this.AddAdjacency(s, d, dir);
                }
            }
        }

        /// <summary>
        /// Declares that dest can be placed adjacent to src, in the direction specified by (x, y, z).
        /// (x, y, z) must be a valid direction, which usually means a unit vector.
        /// </summary>
        public void AddAdjacency(Tile src, Tile dest, int x, int y, int z)
        {
            this.RequireDirections();
            var d = this.directions.GetDirection(x, y, z);
            this.AddAdjacency(src, dest, d);
        }

        /// <summary>
        /// Declares that dest can be placed adjacent to src, in the direction specified.
        /// </summary>
        public void AddAdjacency(Tile src, Tile dest, Direction d)
        {
            var id = this.directions.Inverse(d);
            var srcPattern = this.GetPattern(src);
            var destPattern = this.GetPattern(dest);
            this.propagator[srcPattern][(int)d].Add(destPattern);
            this.propagator[destPattern][(int)id].Add(srcPattern);
        }

        public void AddAdjacency(Adjacency adjacency)
        {
            this.AddAdjacency(adjacency.Src, adjacency.Dest, adjacency.Direction);
        }

        public bool IsAdjacent(Tile src, Tile dest, Direction d)
        {
            var srcPattern = this.GetPattern(src);
            var destPattern = this.GetPattern(dest);
            return this.propagator[srcPattern][(int)d].Contains(destPattern);
        }

        public void AddSample(ITopoArray<Tile> sample, TileRotation tileRotation = null)
        {
            foreach (var s in OverlappingAnalysis.GetRotatedSamples(sample, tileRotation))
            {
                this.AddSample(s);
            }
        }

        public void AddSample(ITopoArray<Tile> sample)
        {
            var topology = sample.Topology.AsGridTopology();

            this.SetDirections(topology.Directions);

            var width = topology.Width;
            var height = topology.Height;
            var depth = topology.Depth;
            var directionCount = topology.Directions.Count;

            for (var z = 0; z < depth; z++)
            {
                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        var index = topology.GetIndex(x, y, z);
                        if (!topology.ContainsIndex(index))
                            continue;

                        // Find the pattern and update the frequency
                        var pattern = this.GetPattern(sample.Get(x, y, z));

                        this.frequencies[pattern] += 1;

                        // Update propagator
                        for (var d = 0; d < directionCount; d++)
                        {
                            int x2, y2, z2;
                            if (topology.TryMove(x, y, z, (Direction)d, out x2, out y2, out z2))
                            {
                                var pattern2 = this.GetPattern(sample.Get(x2, y2, z2));
                                this.propagator[pattern][d].Add(pattern2);
                            }
                        }
                    }
                }
            }
        }

        internal override TileModelMapping GetTileModelMapping(ITopology topology)
        {
            var gridTopology = topology.AsGridTopology();
            this.RequireDirections();
            this.SetDirections(gridTopology.Directions);

            if(this.frequencies.Sum() == 0.0)
            {
                throw new Exception("No tiles have assigned frequences.");
            }

            var patternModel = new PatternModel
            {
                Propagator = this.propagator.Select(x => x.Select(y => y.ToArray()).ToArray()).ToArray(),
                Frequencies = this.frequencies.ToArray()
            };
            var tilesToPatternsByOffset = new Dictionary<int, IReadOnlyDictionary<Tile, ISet<int>>>()
                {
                    {0, this.tilesToPatterns.ToDictionary(kv => kv.Key, kv => (ISet<int>)new HashSet<int>{ kv.Value }) }
                };
            var patternsToTilesByOffset = new Dictionary<int, IReadOnlyDictionary<int, Tile>>
                {
                    {0, this.tilesToPatterns.ToDictionary(x => x.Value, x => x.Key)}
                };
            return new TileModelMapping
            {
                PatternTopology = gridTopology,
                PatternModel = patternModel,
                PatternsToTilesByOffset = patternsToTilesByOffset,
                TilesToPatternsByOffset = tilesToPatternsByOffset,
                TileCoordToPatternCoordIndexAndOffset = null
            };
        }

        public override void MultiplyFrequency(Tile tile, double multiplier)
        {
            var pattern = this.tilesToPatterns[tile];
            this.frequencies[pattern] *= multiplier;
        }

        private int GetPattern(Tile tile)
        {
            var directionCount = this.directions.Count;

            int pattern;
            if (!this.tilesToPatterns.TryGetValue(tile, out pattern))
            {
                pattern = this.tilesToPatterns[tile] = this.tilesToPatterns.Count;
                this.frequencies.Add(0);
                this.propagator.Add(new HashSet<int>[directionCount]);
                for (var d = 0; d < directionCount; d++)
                {
                    this.propagator[pattern][d] = new HashSet<int>();
                }
            }
            return pattern;
        }

        public class Adjacency
        {
            public Tile[] Src { get; set; }
            public Tile[] Dest { get; set; }
            public Direction Direction { get; set; }
        }
    }
}
