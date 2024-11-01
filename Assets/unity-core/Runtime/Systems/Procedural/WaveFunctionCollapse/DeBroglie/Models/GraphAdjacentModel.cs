using DeBroglie.Rot;
using DeBroglie.Topo;
using DeBroglie.Wfc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeBroglie.Models
{
    /// <summary>
    /// Functions as AdjacentModel, but is more generic and will function for any toplogy.
    /// </summary>
    public class GraphAdjacentModel : TileModel
    {
        private readonly int directionsCount;
        private readonly int edgeLabelCount;
        private Dictionary<Tile, int> tilesToPatterns;
        private List<double> frequencies;
        // By Pattern, then edge-label
        private List<HashSet<int>[]> propagator;
        private (Direction, Direction, Rotation)[] edgeLabelInfo;

        public GraphAdjacentModel(GraphInfo graphInfo)
            :this(graphInfo.DirectionsCount, graphInfo.EdgeLabelCount)
        {
            this.edgeLabelInfo = graphInfo.EdgeLabelInfo;
        }


        public GraphAdjacentModel(int directionsCount, int edgeLabelCount)
        {
            this.directionsCount = directionsCount;
            this.edgeLabelCount = edgeLabelCount;
            // Tiles map 1:1 with patterns
            this.tilesToPatterns = new Dictionary<Tile, int>();
            this.frequencies = new List<double>();
            this.propagator = new List<HashSet<int>[]>();
        }

        internal override TileModelMapping GetTileModelMapping(ITopology topology)
        {
            if (this.frequencies.Sum() == 0.0)
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
                    {0, this.tilesToPatterns.ToLookup(x=>x.Key, x=>x.Value).ToDictionary(g=>g.Key, g=>(ISet<int>)new HashSet<int>(g)) }
                };
            var patternsToTilesByOffset = new Dictionary<int, IReadOnlyDictionary<int, Tile>>
                {
                    {0, this.tilesToPatterns.ToDictionary(x => x.Value, x => x.Key)}
                };
            return new TileModelMapping
            {
                PatternTopology = topology,
                PatternModel = patternModel,
                PatternsToTilesByOffset = patternsToTilesByOffset,
                TilesToPatternsByOffset = tilesToPatternsByOffset,
                TileCoordToPatternCoordIndexAndOffset = null
            };
        }

        public override IEnumerable<Tile> Tiles => this.tilesToPatterns.Keys;

        public override void MultiplyFrequency(Tile tile, double multiplier)
        {
            var pattern = this.tilesToPatterns[tile];
            this.frequencies[pattern] *= multiplier;
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
            foreach (var tile in this.Tiles)
            {
                this.SetFrequency(tile, 1.0);
            }
        }

        private int GetPattern(Tile tile)
        {
            int pattern;
            if (!this.tilesToPatterns.TryGetValue(tile, out pattern))
            {
                pattern = this.tilesToPatterns[tile] = this.tilesToPatterns.Count;
                this.frequencies.Add(0);
                this.propagator.Add(new HashSet<int>[this.edgeLabelCount]);
                for (var el = 0; el < this.edgeLabelCount; el++)
                {
                    this.propagator[pattern][el] = new HashSet<int>();
                }
            }
            return pattern;
        }

        public void AddAdjacency(IList<Tile> src, IList<Tile> dest, Direction direction, TileRotation tileRotation)
        {
            foreach (var s in src)
            {
                foreach (var d in dest)
                {
                    this.AddAdjacency(s, d, direction, tileRotation);
                }
            }
        }

        public void AddAdjacency(Tile src, Tile dest, Direction direction, TileRotation tileRotation)
        {
            if(this.edgeLabelInfo == null)
            {
                throw new Exception("This method requires edgeLabelInfo configured");
            }
            var inverseDirectionItems = this.edgeLabelInfo.Where(x => x.Item3.IsIdentity && x.Item1 == direction).ToList();
            if(inverseDirectionItems.Count == 0)
            {
                throw new Exception($"Couldn't find identity edge label for direction {direction}");
            }
            var inverseDirection = inverseDirectionItems[0].Item2;
            for (var i = 0; i < this.edgeLabelInfo.Length; i++)
            {
                var (d, _, r) = this.edgeLabelInfo[i];
                if (d == direction)
                {
                    var rotation = r.Inverse();
                    if (tileRotation.Rotate(dest, rotation, out var rd))
                    {
                        this.AddAdjacency(src, rd, (EdgeLabel)i);
                    }
                }
                if (d == inverseDirection)
                {
                    var rotation = r.Inverse();
                    if (tileRotation.Rotate(src, rotation, out var rs))
                    {
                        this.AddAdjacency(dest, rs, (EdgeLabel)i);
                    }
                }
            }
        }


        public void AddAdjacency(IList<Tile> src, IList<Tile> dest, EdgeLabel edgeLabel)
        {
            foreach(var s in src)
            {
                foreach(var d in dest)
                {
                    this.AddAdjacency(s, d, edgeLabel);
                }
            }
        }

        public void AddAdjacency(Tile src, Tile dest, EdgeLabel edgeLabel)
        {
            var s = this.GetPattern(src);
            var d = this.GetPattern(dest);
            this.propagator[s][(int)edgeLabel].Add(d);
        }

        public bool IsAdjacent(Tile src, Tile dest, EdgeLabel edgeLabel)
        {
            var srcPattern = this.GetPattern(src);
            var destPattern = this.GetPattern(dest);
            return this.propagator[srcPattern][(int)edgeLabel].Contains(destPattern);
        }
    }
}
