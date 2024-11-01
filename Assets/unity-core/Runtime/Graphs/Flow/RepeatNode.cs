using System;
using System.Collections;
using Nodes.Value;

namespace Nodes.Branch
{
    [Serializable]
    public class RepeatNode : FlowNode
    {
        [Parameter]
        public ValueNode<bool> Condition;

        [Output]
        public Node Out;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            while (this.Condition?.GetValue() ?? false)
            {
                yield return this.Out?.ExcuteAsync(context);
            }
        }
    }
}