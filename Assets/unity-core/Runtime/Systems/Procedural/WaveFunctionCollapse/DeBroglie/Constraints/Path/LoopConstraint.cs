using DeBroglie.Topo;
using System.Collections.Generic;
using System.Linq;

namespace DeBroglie.Constraints
{
    /// <summary>
    /// Enforces that the entire path is made out of loops,
    /// i.e. there are at least two routes between any two connected points.
    /// </summary>
    public class LoopConstraint : ITileConstraint
    {
        private IPathView pathView;

        public IPathSpec PathSpec { get; set; }

        public void Init(TilePropagator propagator)
        {
            if (this.PathSpec is PathSpec pathSpec)
            {
                // Convert PathSpec to EdgedPathSpec
                // As we have a bug with PathSpec ignoring paths of length 2.
                // (probably should use bridge edges instead of articulation points)
                ISet<Direction>  allDirections = new HashSet<Direction>(Enumerable.Range(0, propagator.Topology.DirectionsCount).Cast<Direction>());
                var edgedPathSpec = new EdgedPathSpec
                {
                    Exits = pathSpec.Tiles.ToDictionary(x => x, _ => allDirections),
                    RelevantCells = pathSpec.RelevantCells,
                    RelevantTiles = pathSpec.RelevantTiles,
                    TileRotation = pathSpec.TileRotation
                };
                this.pathView = edgedPathSpec.MakeView(propagator);
            }
            else
            {
                this.pathView = this.PathSpec.MakeView(propagator);
            }

            this.pathView.Init();
        }

        public void Check(TilePropagator propagator)
        {
            this.pathView.Update();

            var info = PathConstraintUtils.GetArticulationPoints(this.pathView.Graph, this.pathView.CouldBePath, this.pathView.MustBeRelevant);
            var isArticulation = info.IsArticulation;

            for (var i = 0; i < this.pathView.Graph.NodeCount; i++)
            {
                if (isArticulation[i])
                {
                    propagator.SetContradiction("Loop constraint found articulation point.", this);
                    return;
                }
            }

        }
    }
}
