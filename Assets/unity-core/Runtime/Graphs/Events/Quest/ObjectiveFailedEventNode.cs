namespace Nodes.Events
{
    using System;

    [Serializable]
    public class ObjectiveFailedEventNode : EventNode<ObjectiveFailedEventParameters>
    {
        [HideLabel]
        public ObjectiveSO Objective;

        protected override bool MatchParameters(ObjectiveFailedEventParameters parameters)
        {
            return this.Objective == parameters.Objective;
        }
    }
}
