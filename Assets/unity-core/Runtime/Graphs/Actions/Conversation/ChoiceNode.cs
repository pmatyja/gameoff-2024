using System;
using System.Collections;

namespace Nodes.Actions.Conversation
{
    [Serializable]
    [MultipleInputs(typeof(ChoiceNode))]
    public class ChoiceNode : Choice, INode, IFlowNode
    {
        public float Width => 288.0f;
        public string BackgroundColor { get; } = "#424c6e";

        [Output]
        public Node Out;

        public IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            if (string.IsNullOrWhiteSpace(this.SetMarker) == false)
            {
                ChoiceTrackerSO.Instance?.MarkChoice(this.SetMarker);
            }

            yield return this.Out?.ExcuteAsync(context);
        }
    }
}
