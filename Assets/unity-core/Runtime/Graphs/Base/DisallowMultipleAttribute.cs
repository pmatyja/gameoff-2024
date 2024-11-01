using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class DisallowMultipleAttribute : Attribute
{
}