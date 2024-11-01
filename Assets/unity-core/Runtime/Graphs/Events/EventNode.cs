public abstract class EventNode : INode
{
    public virtual float Width => 208;
    public virtual string BackgroundColor { get; } = "#8C4134";
}

public abstract class EventNode<T> : EventNode where T : struct
{
    [Output]
    public Node Trigger;

    protected abstract bool MatchParameters(T parameters);

    protected void OnEvent(object sender, T parameters)
    {
        CoroutineManager.Start(this.Trigger?.ExcuteAsync(new NodeGraphContext()), nameof(EventNode<T>));
    }

    protected void OnEnable()
    {
        EventBus.AddListener<T>(this.OnEvent, this.MatchParameters);
    }

    protected virtual void OnDisable()
    {
        EventBus.RemoveListener<T>(this.OnEvent);
    }
}
