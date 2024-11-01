public class EnableIfAttribute : DependsOnAttribute
{
    public EnableIfAttribute(string field, params object[] values)
        : base(field, DependencyAction.Enable, values)
    {
    }
}