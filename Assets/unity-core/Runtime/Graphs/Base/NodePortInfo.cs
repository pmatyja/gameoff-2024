using System;
using System.Reflection;
using UnityEngine;

public sealed class NodePortInfo
{
    public string Name { get; set; }
    public Color Color { get; set; }
    public Type Type { get; set; }
    public bool Input { get; set; }
    public bool Single { get; set; }
    public FieldInfo Field { get; set; }

    public override string ToString()
    {
        if (this.Input)
        {
            if (this.Single)
            {
                return $"in {this.Name}";
            }

            return $"in {this.Name}<>";
        }

        if (this.Single)
        {
            return $"out {this.Name}";
        }

        return $"out {this.Name}<>";
    }
}