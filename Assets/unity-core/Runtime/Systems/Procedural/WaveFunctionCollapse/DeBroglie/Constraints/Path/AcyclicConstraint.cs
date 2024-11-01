using System.Collections.Generic;

namespace DeBroglie.Constraints
{
    /// <summary>
    /// Enforces that there are no loops at all
    /// </summary>
    public class AcyclicConstraint : ITileConstraint
    {
        private IPathView pathView;

        public IPathSpec PathSpec { get; set; }

        public void Init(TilePropagator propagator)
        {
            this.pathView = this.PathSpec.MakeView(propagator);
            this.pathView.Init();
        }

        public void Check(TilePropagator propagator)
        {
            this.pathView.Update();

            var graph = this.pathView.Graph;
            var mustBePath = this.pathView.MustBePath;
            // TODO: Support relevant?
            var visited = new bool[graph.NodeCount];
            for (var i = 0; i < graph.NodeCount; i++)
            {
                if (!mustBePath[i]) continue;
                if (visited[i]) continue;

                // Start DFS
                var stack = new Stack<(int, int)>();
                stack.Push((-1, i));
                while(stack.Count > 0)
                {
                    var (prev, u) = stack.Pop();
                    if(visited[u])
                    {
                        propagator.SetContradiction("Acyclic constraint found cycle", this);
                        return;
                    }
                    visited[u] = true;
                    foreach(var v in graph.Neighbours[u])
                    {
                        if (!mustBePath[v]) continue;
                        if (v == prev) continue;
                        stack.Push((u, v));
                    }
                }
            }

        }
    }
}
