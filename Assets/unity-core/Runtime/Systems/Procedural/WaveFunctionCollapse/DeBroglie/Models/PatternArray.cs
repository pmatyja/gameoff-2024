namespace DeBroglie.Models
{
    internal struct PatternArray
    {
        public Tile[,,] Values;

        public int Width
        {
            get { return this.Values.GetLength(0); }
        }

        public int Height
        {
            get { return this.Values.GetLength(1); }
        }

        public int Depth
        {
            get { return this.Values.GetLength(2); }
        }
    }

}
