public interface INode
{
    float Width { get; }
    string BackgroundColor { get; }
    
    public virtual void OnEnable()
    {
    }
    
    public virtual void OnDisable()
    {
    }
}