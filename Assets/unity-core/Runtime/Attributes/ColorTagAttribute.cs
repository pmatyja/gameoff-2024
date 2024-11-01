using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class ColorTagAttribute : BaseAttribute
{
    public readonly Color Color;

    public ColorTagAttribute(byte r, byte g, byte b)
    {
        this.Color = new Color(r / 255.0f, g / 255.0f, b / 255.0f);
    }

    public ColorTagAttribute(float r, float g, float b)
    {
        this.Color = new Color(r, g, b);
    }
}
