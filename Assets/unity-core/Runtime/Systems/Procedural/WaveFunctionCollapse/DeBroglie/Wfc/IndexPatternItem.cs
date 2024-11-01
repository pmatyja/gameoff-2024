using System;

namespace DeBroglie.Wfc
{
    internal struct IndexPatternItem : IEquatable<IndexPatternItem>
    {
        public int Index { get; set; }
        public int Pattern { get; set; }

        public bool Equals(IndexPatternItem other)
        {
            return other.Index == this.Index && other.Pattern == this.Pattern;
        }

        public override bool Equals(object obj)
        {
            if (obj is IndexPatternItem other)
            {
                return this.Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.Index.GetHashCode() * 17 + this.Pattern.GetHashCode();
            }
        }

        public static bool operator ==(IndexPatternItem a, IndexPatternItem b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(IndexPatternItem a, IndexPatternItem b)
        {
            return !a.Equals(b);
        }
    }
}
