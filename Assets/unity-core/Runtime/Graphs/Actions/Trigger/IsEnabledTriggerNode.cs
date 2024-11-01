using System;
using System.Collections;

namespace Nodes.Actions.Triggers
{
    [Serializable]
    public class IsEnabledTriggerNode : ActionNode
    {
        [HideLabel]
        public Trigger Target;
        public bool State;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            this.Target.IsEnabled = this.State;
            yield return base.ExcuteAsync(context);
        }
    }
}