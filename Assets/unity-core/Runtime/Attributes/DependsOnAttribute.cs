public abstract class DependsOnAttribute : BaseAttribute
{
    public readonly string Field;
    public readonly object[] Values;
    public readonly DependencyAction Action;

    public enum DependencyAction
    {
        Enable,
        Disable,
        Show,
        Hide
    }

    protected DependsOnAttribute(string field, DependencyAction action, params object[] values)
    {
        this.Field = field;
        this.Action = action;
        this.Values = values;
    }
}