using System;
using System.Collections;

namespace Nodes.Actions.Conversation
{
    [Serializable]
    public class ChoiceSetMarkerNode : ActionNode
    {
        public override float Width => 200.0f;
        public override string BackgroundColor { get; } = "#424c6e";

        [ChoiceTrackerSelector(Label = LabelState.Hidden)]
        public string Marker;

        public bool State;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            ChoiceTrackerSO.Instance?.MarkChoice(Marker);
            yield return base.ExcuteAsync(context);
        }
    }
}
