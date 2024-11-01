using System;
using System.Collections;

namespace Nodes.Actions.Quest
{
    [Serializable]
    public class QuestSucceedesNode : EventProducerNode
    {
        [HideLabel]
        public QuestSO Quest;
        public bool FailRemainingObjectives;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            this.RaiseEvent(new QuestSucceededEventParameters
            {
                Quest = this.Quest,
                FailRemainingObjectives = this.FailRemainingObjectives
            });

            yield return base.ExcuteAsync(context);
        }
    }
}