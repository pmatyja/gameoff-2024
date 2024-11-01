using System;
using System.Collections;

namespace Nodes.Actions.Quest
{
    [Serializable]
    public class WaitForQuestStatus : ActionNode
    {
        [HideLabel]
        public QuestSO Quest;
        
        [HideLabel]
        public ProgressStatus Status;

        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            if (this.Quest == null)
            {
                yield break;
            }

            yield return Wait.UntilFalse(() => this.Quest.Status == this.Status);
            yield return base.ExcuteAsync(context);
        }
    }
}