using System;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
public sealed class DependencyAttribute : Attribute
{
    public string ModuleId { get; }
    public int Major { get; }
    public int Minor { get; }

    public DependencyAttribute(string moduleId, int major, int minor)
    {
        this.ModuleId = moduleId;
        this.Major = major;
        this.Minor = minor;
    }
}
