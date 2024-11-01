using System;
using System.Collections;
using Nodes.Value;

namespace Nodes.Branch
{
    [Serializable]
    public class RepeatCountNode : FlowNode
    {
        [Parameter]
        public ValueNode<int> Count;

        [Output]
        public Node Out;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            var count = 0;

            while (count++ < (this.Count?.GetValue() ?? default))
            {
                yield return this.Out?.ExcuteAsync(context);
            }
        }
    }
}