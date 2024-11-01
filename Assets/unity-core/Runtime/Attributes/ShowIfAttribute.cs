public class ShowIfAttribute : DependsOnAttribute
{
    public ShowIfAttribute(string field, params object[] values)
        : base(field, DependencyAction.Show, values)
    {
    }
}