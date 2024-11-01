namespace DeBroglie.Rot
{
    /// <summary>
    /// Represents a tile that has been rotated and reflected in some way.
    /// </summary>
    public struct RotatedTile
    {
        public Tile Tile { get; set; }
        public Rotation Rotation { get; set; }

        public RotatedTile(Tile tile, Rotation rotation)
        {
            this.Tile = tile;
            this.Rotation = rotation;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + this.Rotation.GetHashCode();
                hash = hash * 23 + this.Tile.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is RotatedTile other)
            {
                return this.Rotation.Equals(other.Rotation) && this.Tile == other.Tile;
            }
            else {
                return base.Equals(obj);
            }
        }

        public override string ToString()
        {
            return this.Tile.ToString() + this.Rotation.ToString();
        }
    }
}
