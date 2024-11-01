using System;
using UnityEngine;

[Serializable]
public class WindProperties
{
    [SerializeField]
    [Range(0.0001f, 100.0f)]
    private float windIntensityFrequency = 20f;
    public float IntensityFrequency { get => this.windIntensityFrequency; set => this.windIntensityFrequency = Mathf.Clamp(value, 0.0f, 100.0f); }

    [SerializeField]
    [Range(0.0f, 5.0f)]
    private float windIntensityMultiplier = 1.0f;
    public float IntensityMultiplier { get => this.windIntensityMultiplier; set => this.windIntensityMultiplier = Mathf.Clamp(value, 0.0f, 5.0f); }

    [SerializeField]
    [Range(0.0001f, 100.0f)]
    private float windDirectionFrequency = 10.0f;
    public float DirectionFrequency { get => this.windDirectionFrequency; set => this.windDirectionFrequency = Mathf.Clamp(value, 0.0f, 100.0f); }
}