namespace Nodes.Events
{
    using System;

    [Serializable]
    public class AreaTriggerEventNode : ChannelEventNode<AreaTriggerEventParameters>
    {
        protected override bool MatchParameters(AreaTriggerEventParameters parameters)
        {
            return true;
        }
    }
}
