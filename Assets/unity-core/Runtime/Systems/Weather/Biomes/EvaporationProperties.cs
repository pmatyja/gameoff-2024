using System;
using UnityEngine;

[Serializable]
public class EvaporationProperties
{
    [SerializeField]
    [Range(0.0f, 5.0f)]
    private float evaporationMultiplier = 1.0f;
    public float EvaporationMultiplier { get => this.evaporationMultiplier; set => this.evaporationMultiplier = Mathf.Clamp(value, 0.0f, 5.0f); }

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float temperatureEvaporationMultiplier = 1.0f;
    public float TemperatureEvaporationMultiplier { get => this.temperatureEvaporationMultiplier; set => this.temperatureEvaporationMultiplier = Mathf.Clamp01(value); }

    [SerializeField]
    [Range(0.0f, 1.0f)]
    public float humidityEvaporationMultiplier = 0.1f;
    public float HumidityEvaporationMultiplier { get => this.humidityEvaporationMultiplier; set => this.humidityEvaporationMultiplier = Mathf.Clamp01(value); }

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float windIntensityEvaporationMultiplier = 0.1f;
    public float WindIntensityEvaporationMultiplier { get => this.windIntensityEvaporationMultiplier; set => this.windIntensityEvaporationMultiplier = Mathf.Clamp01(value); }

    [SerializeField]
    [Range(0.0f, 1.0f)]
    public float transpiration = 0.1f;
    public float Transpiration { get => this.transpiration; set => this.transpiration = Mathf.Clamp01(value); }
}