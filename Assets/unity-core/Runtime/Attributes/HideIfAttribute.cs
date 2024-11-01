public class HideIfAttribute : DependsOnAttribute
{
    public HideIfAttribute(string field, params object[] values)
        : base(field, DependencyAction.Hide, values)
    {
    }
}