using System;
using System.Collections;

[Serializable]
public abstract class ChannelEventProducerNode : ActionNode
{
    public override float Width => 288.0f;

    [HideLabel]
    public EventChannelSO Channel;

    protected void RaiseEvent<T>(T parameters) where T : struct
    {
        EventBus.Raise(this, parameters);
    }
}