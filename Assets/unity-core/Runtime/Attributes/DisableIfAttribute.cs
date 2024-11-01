public class DisableIfAttribute : DependsOnAttribute
{
    public DisableIfAttribute(string field, params object[] values) 
        : base(field, DependencyAction.Disable, values)
    {
    }
}