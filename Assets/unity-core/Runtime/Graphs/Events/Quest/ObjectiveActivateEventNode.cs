namespace Nodes.Events
{
    using System;

    [Serializable]
    public class ObjectiveActivateEventNode : EventNode<ObjectiveActivateEventParameters>
    {
        [HideLabel]
        public ObjectiveSO Objective;

        protected override bool MatchParameters(ObjectiveActivateEventParameters parameters)
        {
            return this.Objective == parameters.Objective;
        }
    }
}
