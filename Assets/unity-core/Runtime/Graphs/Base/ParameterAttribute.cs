using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class ParameterAttribute : Attribute
{
    public bool IsOptional;
}