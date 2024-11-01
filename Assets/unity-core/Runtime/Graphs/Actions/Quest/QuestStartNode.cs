using System;
using System.Collections;

namespace Nodes.Actions.Quest
{
    [Serializable]
    public class QuestStartNode : EventProducerNode
    {
        [HideLabel]
        public QuestSO Quest;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            this.RaiseEvent(new QuestStartEventParameters
            {
                Quest = this.Quest
            });

            yield return base.ExcuteAsync(context);
        }
    }
}