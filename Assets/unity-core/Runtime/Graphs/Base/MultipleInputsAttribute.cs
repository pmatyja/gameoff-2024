using System;

public sealed class MultipleInputsAttribute : InputAttribute
{
    public MultipleInputsAttribute(Type type) : base(type)
    {
    }
}
