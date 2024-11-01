using System;
using System.Collections;

namespace Nodes.Actions.Quest
{
    [Serializable]
    public class ObjectiveSuccedeNode : EventProducerNode
    {
        [HideLabel]
        public ObjectiveSO Objective;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            this.RaiseEvent(new ObjectiveSucceededEventParameters
            {
                Objective = this.Objective
            });

            yield return base.ExcuteAsync(context);
        }
    }
}