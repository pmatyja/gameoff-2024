using System;
using UnityEngine;

[Serializable]
public class TemperatureProperties
{
    public AnimationCurve TemperatureCurve = new
    (
        new Keyframe(0.0f, 0.0f, 4.0f, 4.0f, 0.0f, 0.0f),
        new Keyframe(0.5f, 1.0f, 0.0f, 0.0f, 0.5f, 0.67f),
        new Keyframe(1.0f, 0.0f, -4.0f, -4.0f, 0.0f, 0.0f)
    );

    [SerializeField]
    [Range(0.0f, 50.0f)]
    private float dailyTemperatureRange = 20.0f;
    public float DailyTemperatureRange { get => this.dailyTemperatureRange; set => this.dailyTemperatureRange = Mathf.Clamp(value, 0.0f, 50.0f); }
    
    [SerializeField]
    [Range(-100.0f, 100.0f)]
    private float yearlyMinTemperature = -20.0f;
    public float YearlyMinTemperature { get => this.yearlyMinTemperature; set => this.yearlyMinTemperature = Mathf.Clamp(value, -100.0f, 100.0f); }
    
    [SerializeField]
    [Range(-100.0f, 100.0f)]
    private float yearlyMaxTemperature = 15.0f;
    public float YearlyMaxTemperature { get => this.yearlyMaxTemperature; set => this.yearlyMaxTemperature = Mathf.Clamp(value, -100.0f, 100.0f); }

    [SerializeField]
    [Range(0.0001f, 100.0f)]
    private float temperatureDeviationFrequency = 20.0f;
    public float TemperatureDeviationFrequency { get => this.temperatureDeviationFrequency; set => this.temperatureDeviationFrequency = Mathf.Clamp(value, 0.0001f, 100.0f); }

    [Range(0.0f, 100.0f)]
    public float temperatureDeviationRange = 10.0f;
    public float TemperatureDeviationRange { get => this.temperatureDeviationRange; set => this.temperatureDeviationRange = Mathf.Clamp(value, 0.0f, 100.0f); }
}