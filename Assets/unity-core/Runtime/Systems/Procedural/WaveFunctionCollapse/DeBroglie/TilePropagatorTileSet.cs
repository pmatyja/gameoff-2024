using System.Collections.Generic;
using System.Linq;

namespace DeBroglie
{
    /// <summary>
    /// A set of tiles, specific to a particular TilePropagator.
    /// This class internally caches some computations, making it faster
    /// if you have lots of operations using the same set of tiles.
    /// </summary>
    public class TilePropagatorTileSet
    {
        internal TilePropagatorTileSet(IEnumerable<Tile> tiles)
        {
            this.Tiles = tiles.ToArray();
            this.OffsetToPatterns = new Dictionary<int, ISet<int>>();
        }

        public IReadOnlyCollection<Tile> Tiles { get; }

        internal Dictionary<int, ISet<int>> OffsetToPatterns { get; }

        public override string ToString()
        {
            return string.Join(",", this.Tiles);
        }
    }
}
