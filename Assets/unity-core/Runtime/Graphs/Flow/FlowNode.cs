namespace Nodes.Branch
{
    public abstract class FlowNode : Node
    {
        public override float Width => 288.0f;
        public override string BackgroundColor { get; } = "#4B4B4B";
    }
}
