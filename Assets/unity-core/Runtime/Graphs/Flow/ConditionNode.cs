namespace Nodes.Branch
{
    using System;
    using System.Collections;
    using Value;

    [Serializable]
    public class ConditionNode : FlowNode
    {
        [Parameter]
        public ValueNode<bool> Condition;

        [Output]
        public Node True;

        [Output]
        public Node False;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            if (this.Condition?.GetValue() ?? false)
            {
                yield return this.True?.ExcuteAsync(context);
            }
            else
            {
                yield return this.False?.ExcuteAsync(context);
            }
        }
    }
}
