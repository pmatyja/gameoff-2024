public abstract class ChannelEventNode : EventNode
{
}

public abstract class ChannelEventNode<T> : ChannelEventNode where T : struct
{
    [HideLabel]
    public EventChannelSO Channel;

    [Output]
    public Node Trigger;

    protected abstract bool MatchParameters(T parameters);

    protected void OnEvent(object sender, T parameters)
    {
        CoroutineManager.Start(this.Trigger?.ExcuteAsync(new NodeGraphContext()), nameof(ChannelEventNode<T>));
    }

    protected void OnEnable()
    {
        EventBus.AddListener<T>(this.OnEvent, this.Channel, this.MatchParameters);
    }

    protected virtual void OnDisable()
    {
        EventBus.RemoveListener<T>(this.OnEvent);
    }
}
