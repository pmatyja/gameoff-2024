namespace Nodes.Events
{
    using System;

    [Serializable]
    public class QuestSucceededEventEventNode : EventNode<QuestSucceededEventParameters>
    {
        [HideLabel]
        public QuestSO Quest;

        protected override bool MatchParameters(QuestSucceededEventParameters parameters)
        {
            return this.Quest == parameters.Quest;
        }
    }
}
