﻿using System.Collections;
using System.Collections.Generic;

namespace DeBroglie.Rot
{
    /// <summary>
    /// Describes a group of rotations and reflections.
    /// </summary>
    public class RotationGroup : IEnumerable<Rotation>
    {
        private readonly int rotationalSymmetry;
        private readonly bool reflectionalSymmetry;
        private readonly int smallestAngle;
        private readonly List<Rotation> rotations;

        public RotationGroup(int rotationalSymmetry, bool reflectionalSymmetry)
        {
            this.rotationalSymmetry = rotationalSymmetry;
            this.reflectionalSymmetry = reflectionalSymmetry;
            this.smallestAngle = 360 / rotationalSymmetry;
            this.rotations = new List<Rotation>();
            for (var refl = 0; refl < (reflectionalSymmetry ? 2 : 1); refl++)
            {
                for (var rot = 0; rot < 360; rot += this.smallestAngle)
                {
                    this.rotations.Add(new Rotation(rot, refl > 0));
                }
            }
        }

        /// <summary>
        /// Indicates the number of distinct rotations in the group.
        /// </summary>
        public int RotationalSymmetry => this.rotationalSymmetry;

        /// <summary>
        /// If true, the group also contains reflections as well as rotations.
        /// </summary>
        public bool ReflectionalSymmetry => this.reflectionalSymmetry;

        /// <summary>
        /// Defined as 360 / RotationalSymmetry, this is the the smallest angle of any rotation
        /// in the group.
        /// </summary>
        public int SmallestAngle => this.smallestAngle;

        /// <summary>
        /// Throws if rotation is not a member of the group.
        /// </summary>
        /// <param name="rotation"></param>
        public void CheckContains(Rotation rotation)
        {
            if(rotation.RotateCw / this.smallestAngle * this.smallestAngle != rotation.RotateCw)
            {
                throw new System.Exception($"Rotation angle {rotation.RotateCw} not permitted.");
            }
            if(rotation.ReflectX && !this.reflectionalSymmetry)
                throw new System.Exception($"Reflections are not permitted.");
        }

        public IEnumerator<Rotation> GetEnumerator()
        {
            return this.rotations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.rotations.GetEnumerator();
        }
    }
}
