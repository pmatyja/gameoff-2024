using System.Collections;

[MultipleInputs(typeof(Node))]
public abstract class Node : INode, IFlowNode
{
    public virtual float Width => 288.0f;
    public abstract string BackgroundColor { get; }


    public abstract IEnumerator ExcuteAsync(INodeGraphContext context);
}