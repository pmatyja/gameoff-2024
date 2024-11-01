using System.Collections;

public interface IFlowNode
{
    IEnumerator ExcuteAsync(INodeGraphContext context);
}