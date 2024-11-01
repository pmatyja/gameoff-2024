using System;

namespace DeBroglie.Rot
{
    /// <summary>
    /// Represents an rotation in the x-y plane.
    /// Despite the fact this is labelled rotation, it also includes reflections as well.
    /// </summary>
    public struct Rotation : IEquatable<Rotation>
    {
        public Rotation(int rotateCw = 0, bool reflectX = false)
        {
            this.RotateCw = rotateCw;
            this.ReflectX = reflectX;
        }

        /// <summary>
        /// Rotation in degrees, clockwise (assuming a y-down co-ordinate system, typically used
        /// for 2d graphics).
        /// </summary>
        public int RotateCw { get; }

        /// <summary>
        /// If true, this "rotation" also includes a reflection along the x-axis.
        /// The reflection is applied before doing any rotation by RotateCw.
        /// </summary>
        public bool ReflectX { get; }

        /// <summary>
        /// True for the default constructed rotation, that doesn't do anything.
        /// </summary>
        public bool IsIdentity => this.RotateCw == 0 && !this.ReflectX;

        /// <summary>
        /// Returns the rotation that rotates back from this one.
        /// i.e. r.Inverse() * r gives the identity rotation for all Rotation objects.
        /// </summary>
        public Rotation Inverse()
        {
            return new Rotation(this.ReflectX ? this.RotateCw : (360 - this.RotateCw) % 360, this.ReflectX);
        }

        /// <summary>
        /// Returns the rotation which is equivalent to rotating first by b, then by a.
        /// </summary>
        public static Rotation operator *(Rotation a, Rotation b)
        {
            return new Rotation(
                ((b.ReflectX ? -a.RotateCw : a.RotateCw) + b.RotateCw + 360) % 360,
                a.ReflectX ^ b.ReflectX);
        }

        public override bool Equals(object obj)
        {
            if (obj is Rotation other)
            {
                return this.Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.RotateCw * 2 + (this.ReflectX ? 1 : 0);
        }

        public override string ToString()
        {
            return "!" + (this.ReflectX ? "x" : "") + (this.RotateCw);
        }

        public bool Equals(Rotation other)
        {
            return this.RotateCw == other.RotateCw && this.ReflectX == other.ReflectX;
        }

        public static bool operator ==(Rotation a, Rotation b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Rotation a, Rotation b)
        {
            return !a.Equals(b);
        }
    }
}
