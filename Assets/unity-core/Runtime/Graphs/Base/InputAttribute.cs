using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class InputAttribute : Attribute
{
    public Type Type { get; }

    protected InputAttribute(Type type)
    {
        this.Type = type;
    }
}
