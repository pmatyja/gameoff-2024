#nullable enable

public sealed class Reference
{
    public readonly Reference? Parent;
    public readonly object Instance;
    public readonly ReferenceInfo Member;

    public Reference()
    {
        this.Parent = null;
        this.Instance = new object();
        this.Member = ReferenceInfo.Empty;
    }

    private Reference(object instance, ReferenceInfo member, Reference? parent = null)
    {
        this.Instance = instance;
        this.Member = member;
        this.Parent = parent;
    }

    public bool SetValue(object? value)
    {
        if (this.Member.SetValue(this.Instance, value))
        {
            if (this.Member.Type.IsValueType)
            {
                return this.Parent?.SetValue(this.Instance) ?? true;
            }

            return true;
        }

        return false;
    }

    public object? GetValue()
    {
        return this.Member.GetValue(this.Instance);
    }
}