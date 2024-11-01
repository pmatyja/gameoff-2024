using System.Collections;
using System.Collections.Generic;

namespace Nodes.BehaviourTree
{
    public class SequenceBehaviourNode : BehaviourNode
    {
        [Output]
        public List<Node> Sequence;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            foreach (var node in this.Sequence)
            {
                yield return node?.ExcuteAsync(context);
            }
        }
    }
}