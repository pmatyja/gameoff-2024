using DeBroglie.Rot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeBroglie.Topo
{
    /// <summary>
    /// Builds a GraphTopology that represents a mesh, i.e. a series of faces that connect to each other along their edges.
    /// </summary>
    public class MeshTopologyBuilder
    {
        private DirectionSet directions;

        private int edgeLabelCount;
        
        // By index, direction
        private GraphTopology.NeighbourDetails[,] neighbours;

        private readonly Dictionary<(int, int), Direction> pendingInverses = new();

        public MeshTopologyBuilder(DirectionSet directions)
        {
            if(directions.Type != DirectionSetType.Cartesian2d)
            {
                throw new NotImplementedException($"Direction type {directions.Type} not supported");
            }
            this.directions = directions;
            this.edgeLabelCount = directions.Count * directions.Count;
            this.neighbours = new GraphTopology.NeighbourDetails[0, directions.Count];
        }

        private int GetAngle(Direction d)
        {
            switch (d)
            {
                case Direction.XPlus: return 0;
                case Direction.YPlus: return 90;
                case Direction.XMinus: return 180;
                case Direction.YMinus: return 270;
            }
            throw new Exception();
        }

        private Rotation GetRotation(Direction direction, Direction inverseDirection)
        {
            return new Rotation((360 + this.GetAngle(direction) - this.GetAngle(inverseDirection) + 180) % 360);
        }

        private EdgeLabel GetEdgeLabel(Direction direction, Direction inverseDirection)
        {
            return (EdgeLabel)((int)direction + this.directions.Count * (int)inverseDirection);
        }

        /// <summary>
        /// Registers face1 and face2 as adjacent, moving in direction from face1 to face2.
        /// If you call this, you will also need to call add with face1 and face2 swapped, to
        /// establish the direction when travelling back.
        /// </summary>
        public void Add(int face1, int face2, Direction direction)
        {
            if (this.pendingInverses.TryGetValue((face2, face1), out var inverseDirection))
            {
                this.Add(face1, face2, direction, inverseDirection);
                this.pendingInverses.Remove((face2, face1));
            }
            else
            {
                this.pendingInverses.Add((face1, face2), direction);
            }
        }

        /// <summary>
        /// Registers face1 and face2 as adjacent, moving in direction from face1 to face2 and inverseDirection from face2 to face1.
        /// </summary>
        public void Add(int face1, int face2, Direction direction, Direction inverseDirection)
        {
            var maxFace = Math.Max(face1, face2);
            if (this.neighbours.GetLength(0) <= maxFace)
            {
                var newNeighbours = new GraphTopology.NeighbourDetails[maxFace + 1, this.directions.Count];
                Array.Copy(this.neighbours, newNeighbours, this.neighbours.Length);
                for(var f = this.neighbours.GetLength(0);f<maxFace+1;f++)
                {
                    for(var d=0;d< this.directions.Count;d++)
                    {
                        newNeighbours[f, d].Index = -1;
                    }
                }

                this.neighbours = newNeighbours;
            }

            this.neighbours[face1, (int)direction] = new GraphTopology.NeighbourDetails
            {
                Index = face2,
                InverseDirection = inverseDirection,
                EdgeLabel = this.GetEdgeLabel(direction, inverseDirection)
            };
            this.neighbours[face2, (int)inverseDirection] = new GraphTopology.NeighbourDetails
            {
                Index = face1,
                InverseDirection = direction,
                EdgeLabel = this.GetEdgeLabel(inverseDirection, direction)
            };
        }

        public GraphTopology GetTopology()
        {
            if(this.pendingInverses.Count > 0)
            {
                var kv = this.pendingInverses.First();
                throw new Exception($"Some face adjacencies have only been added in one direction, e.g. {kv.Key.Item1} -> {kv.Key.Item2}");
            }
            return new GraphTopology(this.neighbours);
        }

        public GraphInfo GetInfo()
        {
            return new GraphInfo
            {
                DirectionsCount = this.directions.Count,
                EdgeLabelCount = this.edgeLabelCount,
                EdgeLabelInfo = (from el in Enumerable.Range(0, this.edgeLabelCount)
                                 let d = (Direction)(el % 4)
                                 let id = (Direction)(el / 4)
                                 select (d, id, this.GetRotation(d, id))).ToArray()
            };
        }
    }
}
