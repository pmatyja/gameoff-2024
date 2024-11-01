using System;
using System.Collections;

namespace Nodes.Actions.Quest
{
    [Serializable]
    public class WaitForObjectiveStatus : ActionNode
    {
        [HideLabel]
        public ObjectiveSO Objective;
        
        [HideLabel]
        public ProgressStatus Status;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            if (this.Objective == null)
            {
                yield break;
            }

            yield return Wait.UntilFalse(() => this.Objective.Status == this.Status);
            yield return base.ExcuteAsync(context);
        }
    }
}