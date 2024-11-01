using System;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
public sealed class TitleAttribute : Attribute
{
    public string Title;

    public TitleAttribute(string title)
    {
        Title = title;
    }
}
