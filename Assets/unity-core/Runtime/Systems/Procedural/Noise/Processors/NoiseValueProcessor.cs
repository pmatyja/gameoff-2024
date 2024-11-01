using System;

[Serializable]
public abstract class NoiseValueProcessor
{
    public bool Enabled = true;
    public abstract float Process(float value, float x, float y = 1.0f);
}