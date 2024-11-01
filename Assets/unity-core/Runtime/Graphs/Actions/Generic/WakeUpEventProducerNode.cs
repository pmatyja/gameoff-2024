using System;
using System.Collections;

namespace Nodes.Actions.Events
{
    [Serializable]
    public class WakeUpEventProducerNode : ChannelEventProducerNode
    {
        public override IEnumerator ExcuteAsync(INodeGraphContext context)
        {
            this.RaiseEvent(new WakeUpEvent());
            yield return base.ExcuteAsync(context);
        }
    }
}