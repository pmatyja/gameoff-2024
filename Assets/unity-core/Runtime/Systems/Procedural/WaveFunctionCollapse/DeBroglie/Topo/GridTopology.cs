namespace DeBroglie.Topo
{
    /// <summary>
    /// A grid topology is a topology with a regular repeating pattern.
    /// It supports more operations than a generic topology.
    /// </summary>
    public class GridTopology : ITopology
    {
        /// <summary>
        /// Constructs a 2d square grid topology of given dimensions and periodicity.
        /// </summary>
        public GridTopology(int width, int height, bool periodic)
            : this(DirectionSet.Cartesian2d, width, height, 1, periodic, periodic, periodic)
        {
        }

        /// <summary>
        /// Constructs a 3d cube grid topology of given dimensions and periodicity.
        /// </summary>
        public GridTopology(int width, int height, int depth, bool periodic)
            : this(DirectionSet.Cartesian3d, width, height, depth, periodic, periodic, periodic)
        {
        }

        /// <summary>
        /// Constructs a 2d topology.
        /// </summary>
        public GridTopology(DirectionSet directions, int width, int height, bool periodicX, bool periodicY, bool[] mask = null)
            : this(directions, width, height, 1, periodicX, periodicY, false, mask)
        {
        }

        /// <summary>
        /// Constructs a topology.
        /// </summary>
        public GridTopology(DirectionSet directions, int width, int height, int depth, bool periodicX, bool periodicY, bool periodicZ, bool[] mask = null)
        {
            this.Directions = directions;
            this.Width = width;
            this.Height = height;
            this.Depth = depth;
            this.PeriodicX = periodicX;
            this.PeriodicY = periodicY;
            this.PeriodicZ = periodicZ;
            this.Mask = mask;
        }

        /// <summary>
        /// Returns a <see cref="GridTopology"/> with the same parameters, but with the specified mask
        /// </summary>
        public GridTopology WithMask(bool[] mask)
        {
            if (this.Width * this.Height * this.Depth != mask.Length)
                throw new System.Exception("Mask size doesn't fit the topology");

            return new GridTopology(this.Directions, this.Width, this.Height, this.Depth, this.PeriodicX, this.PeriodicY, this.PeriodicZ, mask);
        }

        ITopology ITopology.WithMask(bool[] mask)
        {
            return this.WithMask(mask);
        }

        /// <summary>
        /// Returns a <see cref="GridTopology"/> with the same parameters, but with the specified mask
        /// </summary>
        public GridTopology WithMask(ITopoArray<bool> mask)
        {
            if (!this.IsSameSize(mask.Topology.AsGridTopology()))
                throw new System.Exception("Mask size doesn't fit the topology");
            var boolMask = new bool[this.Width * this.Height * this.Depth];
            for (var z = 0; z < this.Depth; z++)
            {
                for (var y = 0; y < this.Height; y++)
                {
                    for (var x = 0; x < this.Width; x++)
                    {
                        boolMask[x + y * this.Width + z * this.Width * this.Height] = mask.Get(x, y, z);
                    }
                }
            }
            return this.WithMask(boolMask);
        }

        /// <summary>
        /// Returns a <see cref="GridTopology"/> with the same parameters, with the dimensions overridden. Any mask is reset.
        /// </summary>
        public GridTopology WithSize(int width, int height, int depth = 1)
        {
            return new GridTopology(this.Directions, width, height, depth, this.PeriodicX, this.PeriodicY, this.PeriodicZ);
        }

        /// <summary>
        /// Returns a <see cref="GridTopology"/> with the same parameters, with the dimensions overridden.
        /// </summary>
        public GridTopology WithPeriodic(bool periodicX, bool periodicY, bool periodicZ = false)
        {
            return new GridTopology(this.Directions, this.Width, this.Height, this.Depth, periodicX, periodicY, periodicZ, this.Mask);
        }

        /// <summary>
        /// Characterizes the adjacency relationship between locations.
        /// </summary>
        public DirectionSet Directions { get; set; }

        /// <summary>
        /// Number of unique directions
        /// </summary>
        public int DirectionsCount => this.Directions.Count;

        /// <summary>
        /// The extent along the x-axis.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The extent along the y-axis.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The extent along the z-axis.
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Does the topology wrap on the x-axis.
        /// </summary>
        public bool PeriodicX { get; set; }

        /// <summary>
        /// Does the topology wrap on the y-axis.
        /// </summary>
        public bool PeriodicY { get; set; }

        /// <summary>
        /// Does the topology wrap on the z-axis.
        /// </summary>
        public bool PeriodicZ { get; set; }

        /// <summary>
        /// A array with one value per index indcating if the value is missing. 
        /// Not all uses of Topology support masks.
        /// </summary>
        public bool[] Mask { get; set; }

        /// <summary>
        /// Number of unique indices (distinct locations) in the topology
        /// </summary>
        public int IndexCount => this.Width * this.Height * this.Depth;

        /// <summary>
        /// Checks if two grids are the same size without regard for masks or periodicity.
        /// </summary>
        public bool IsSameSize(GridTopology other)
        {
            return this.Width == other.Width && this.Height == other.Height && this.Depth == other.Depth;
        }

        /// <summary>
        /// Reduces a three dimensional co-ordinate to a single integer. This is mostly used internally.
        /// </summary>
        public int GetIndex(int x, int y, int z)
        {
            return x + y * this.Width + z * this.Width * this.Height;
        }

        /// <summary>
        /// Inverts <see cref="GetIndex(int, int, int)"/>
        /// </summary>
        public void GetCoord(int index, out int x, out int y, out int z)
        {
            x = index % this.Width;
            var i = index / this.Width;
            y = i % this.Height;
            z = i / this.Height;
        }

        public bool TryMove(int index, Direction direction, out int dest, out Direction inverseDirection, out EdgeLabel edgeLabel)
        {
            inverseDirection = this.Directions.Inverse(direction);
            edgeLabel = (EdgeLabel)(Direction)direction;
            return this.TryMove(index, direction, out dest);
        }

        public bool TryMove(int index, Direction direction, out int dest)
        {
            int x, y, z;
            this.GetCoord(index, out x, out y, out z);
            return this.TryMove(x, y, z, direction, out dest);
        }

        public bool TryMove(int x, int y, int z, Direction direction, out int dest, out Direction inverseDirection, out EdgeLabel edgeLabel)
        {
            inverseDirection = this.Directions.Inverse(direction);
            edgeLabel = (EdgeLabel)(Direction)direction;
            return this.TryMove(x, y, z, direction, out dest);
        }

        /// <summary>
        /// Given a co-ordinate and a direction, gives the index that is one step in that direction,
        /// if it exists and is not masked out. Otherwise, it returns false.
        /// </summary>
        public bool TryMove(int x, int y, int z, Direction direction, out int dest)
        {
            if (this.TryMove(x, y, z, direction, out x, out y, out z))
            {
                dest = this.GetIndex(x, y, z);
                return true;
            }
            else
            {
                dest = -1;
                return false;
            }
        }

        /// <summary>
        /// Given a co-ordinate and a direction, gives the co-ordinate that is one step in that direction,
        /// if it exists and is not masked out. Otherwise, it returns false.
        /// </summary>
        public bool TryMove(int x, int y, int z, Direction direction, out int destx, out int desty, out int destz)
        {
            var d = (int)direction;
            x += this.Directions.DX[d];
            y += this.Directions.DY[d];
            z += this.Directions.DZ[d];
            if (this.PeriodicX)
            {
                if (x < 0) x += this.Width;
                if (x >= this.Width) x -= this.Width;
            }
            else if (x < 0 || x >= this.Width)
            {
                destx = -1;
                desty = -1;
                destz = -1;
                return false;
            }
            if (this.PeriodicY)
            {
                if (y < 0) y += this.Height;
                if (y >= this.Height) y -= this.Height;
            }
            else if (y < 0 || y >= this.Height)
            {
                destx = -1;
                desty = -1;
                destz = -1;
                return false;
            }
            if (this.PeriodicZ)
            {
                if (z < 0) z += this.Depth;
                if (z >= this.Depth) z -= this.Depth;
            }
            else if (z < 0 || z >= this.Depth)
            {
                destx = -1;
                desty = -1;
                destz = -1;
                return false;
            }
            destx = x;
            desty = y;
            destz = z;
            if (this.Mask != null)
            {
                var index2 = this.GetIndex(x, y, z);
                return this.Mask[index2];
            }
            else
            {
                return true;
            }
        }
    }
}
