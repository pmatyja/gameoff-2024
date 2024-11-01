using UnityEngine;

public class SandstormWeatherEffect : WeatherEffect
{
    public override bool IsActive
    {
        get
        {
            return this.WeatherSystem.Temperature > this.MinTemperature &&
                   this.WeatherSystem.Humidity < this.MaxHumidity && 
                   this.WeatherSystem.WindIntensity > this.MinWindIntensity;
        }
    }

    [Header("Requirements")]
    [SerializeField]
    private float MinTemperature = 35.0f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float MaxHumidity = 0.3f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float MinWindIntensity = 0.5f;

    public SandstormWeatherEffect()
    {
        this.ParticleCount = 300;
    }

    protected override void UpdateEffect()
    {
    }
}
