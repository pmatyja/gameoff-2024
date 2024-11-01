namespace Nodes.Events
{
    using System;

    [Serializable]
    public class ObjectiveSucceededEventNode : EventNode<ObjectiveSucceededEventParameters>
    {
        [HideLabel]
        public ObjectiveSO Objective;

        protected override bool MatchParameters(ObjectiveSucceededEventParameters parameters)
        {
            return this.Objective == parameters.Objective;
        }
    }
}
