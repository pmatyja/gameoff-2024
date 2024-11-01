using UnityEngine;

public class SnowWeatherEffect : WeatherEffect
{
    public override bool IsActive => this.isActive;

    [Header("In Requirements")]
    [SerializeField]
    private float MaxTemperature = 0.5f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float MinHumidity = 0.9f;

    [Header("Out Requirements")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float DesiredHumidity = 0.4f;

    [Header("Properties")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float CondesentionRate = 0.001f;

    private bool isActive;

    public SnowWeatherEffect()
    {
        this.ParticleCount = 2000;
    }

    protected override void UpdateEffect()
    {
        if (this.IsActive)
        {
            this.WeatherSystem.Humidity -= this.CondesentionRate * this.EmissionWeight * Time.deltaTime;
            this.isActive = this.WeatherSystem.Temperature < this.MaxTemperature && this.WeatherSystem.Humidity > this.DesiredHumidity;
        }
        else
        {
            this.isActive = this.WeatherSystem.Temperature < this.MaxTemperature && this.WeatherSystem.Humidity > this.MinHumidity;
        }
    }
}
