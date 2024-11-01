namespace Nodes.Events
{
    using System;

    [Serializable]
    public class QuestStartEventNode : EventNode<QuestStartEventParameters>
    {
        [HideLabel]
        public QuestSO Quest;

        protected override bool MatchParameters(QuestStartEventParameters parameters)
        {
            return this.Quest == parameters.Quest;
        }
    }
}
