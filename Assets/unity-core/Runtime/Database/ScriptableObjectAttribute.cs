using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class ScriptableObjectAttribute : Attribute
{
    public string AssetPrefix { get; }

    public ScriptableObjectAttribute(string assetPrefix)
    {
        this.AssetPrefix = assetPrefix;
    }
}
