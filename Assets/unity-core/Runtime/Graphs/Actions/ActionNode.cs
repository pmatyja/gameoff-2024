using System.Collections;

public abstract class ActionNode : Node
{
    public override float Width => 400.0f;
    public override string BackgroundColor { get; } = "#385980";

    [Output]
    public Node Out;

    public override IEnumerator ExcuteAsync(INodeGraphContext context)
    {
        yield return this.Out?.ExcuteAsync(context);
    }
}
