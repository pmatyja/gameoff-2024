using System;
using DeBroglie.Constraints;

[Serializable]
public struct BorderPrefabConstraint : IPrefabConstraint
{
    public bool Enabled;
    public BorderSides Sides;
    public BorderSides ExcludeSides;
    public bool InvertArea;
    public bool Ban;
}
