using System;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
public sealed class CategoryAttribute : Attribute
{
    public string Category { get; }

    public CategoryAttribute(string category)
    {
        this.Category = category;
    }
}
