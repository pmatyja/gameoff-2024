namespace Nodes.Events
{
    using System;

    [Serializable]
    public class QuestFailedEventEventNode : EventNode<QuestFailedEventParameters>
    {
        [HideLabel]
        public QuestSO Quest;

        protected override bool MatchParameters(QuestFailedEventParameters parameters)
        {
            return this.Quest == parameters.Quest;
        }
    }
}
