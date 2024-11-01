namespace Nodes.Events
{
    using System;

    [Serializable]
    public class ObjectiveAdvanceEventNode : EventNode<ObjectiveAdvanceEventParameters>
    {
        [HideLabel]
        public ObjectiveSO Objective;

        protected override bool MatchParameters(ObjectiveAdvanceEventParameters parameters)
        {
            return this.Objective == parameters.Objective;
        }
    }
}
