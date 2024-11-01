using System;
using System.Collections;

namespace Nodes.Actions.Quest
{
    [Serializable]
    public class ObjectiveFailNode : EventProducerNode
    {
        [HideLabel]
        public ObjectiveSO Objective;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            this.RaiseEvent(new ObjectiveFailedEventParameters
            {
                Objective = this.Objective
            });

            yield return base.ExcuteAsync(context);
        }
    }
}