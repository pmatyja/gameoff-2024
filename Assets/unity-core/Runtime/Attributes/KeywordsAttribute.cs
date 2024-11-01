using System;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
public sealed class KeywordsAttribute : Attribute
{
    public string[] Keywords { get; }

    public KeywordsAttribute(params string[] keywords)
    {
        this.Keywords = keywords;
    }
}
