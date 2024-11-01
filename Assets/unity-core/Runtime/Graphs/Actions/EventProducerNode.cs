public abstract class EventProducerNode : ActionNode
{
    protected void RaiseEvent<T>(T parameters) where T : struct
    {
        EventBus.Raise(this, parameters);
    }
}
