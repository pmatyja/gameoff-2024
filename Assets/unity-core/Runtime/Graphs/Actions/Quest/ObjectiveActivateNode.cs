using System;
using System.Collections;

namespace Nodes.Actions.Quest
{
    [Serializable]
    public class ObjectiveActivateNode : EventProducerNode
    {
        [HideLabel]
        public ObjectiveSO Objective;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            this.RaiseEvent(new ObjectiveActivateEventParameters
            {
                Objective = this.Objective
            });

            yield return base.ExcuteAsync(context);
        }
    }
}