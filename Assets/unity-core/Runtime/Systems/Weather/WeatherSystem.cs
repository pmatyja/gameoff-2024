using UnityEngine;

[DisallowMultipleComponent]
public class WeatherSystem : Singleton<WeatherSystem>
{
    public Transform Camera;

    public Vector3 CameraOffset;

    [Header("Settings")]

    public bool Enabled;

    public WeatherSubsystem Subsystems = (WeatherSubsystem)0x7FFFFFFF;

    [SerializeField]
    [Expandable]
    public BiomeSO Biome;

    [Header("Day / Night cycle")]

    [SerializeField]
    [Range(1.0f, 86400.0f)]
    private float dayLengthInSeconds = 600.0f;
    public float DayLengthInSeconds { get => this.dayLengthInSeconds; set => this.dayLengthInSeconds = Mathf.Clamp(value, 1.0f, 86400.0f); }

    [SerializeField]
    [DisableIf(nameof(Subsystems), WeatherSubsystem.Time)]
    [Range(0.0f, 1.0f)]
    private float timeOfDay = 0.5f;
    public float TimeOfDay { get => this.timeOfDay; set => this.timeOfDay = Mathf.Clamp01(value); }

    [SerializeField]
    [DisableIf(nameof(Subsystems), WeatherSubsystem.Time)]
    [Range(0.0f, 1.0f)]
    private float timeOfYear = 0.5f;
    public float TimeOfYear { get => this.timeOfYear; set => this.timeOfYear = Mathf.Clamp01(value); }

    public Gradient AmbientColor = new()
    { 
        colorKeys = new[]
        { 
            new GradientColorKey(new Color(0.44f, 0.63f, 1.0f, 1.0f), 0.0f), 
            new GradientColorKey(new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.3f), 
            new GradientColorKey(new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.7f), 
            new GradientColorKey(new Color(0.44f, 0.63f, 1.0f, 1.0f), 1.0f)
        }
    };

    [Header("Calendar")]

    [SerializeField]
    [DisableIf(nameof(Subsystems), WeatherSubsystem.Time)]
    [Range(1, 36)]
    private float monthsInYear = 12;
    public float MonthsInYear { get => this.monthsInYear; set => this.monthsInYear = Mathf.Clamp(value, 1, 36); }

    [SerializeField]
    [DisableIf(nameof(Subsystems), WeatherSubsystem.Time)]
    [Range(1, 1000)]
    private float daysInYear = 365;
    public float DaysInYear { get => this.daysInYear; set => this.daysInYear = Mathf.Clamp(value, 1, 1000); }

    [SerializeField]
    [DisableIf(nameof(Subsystems), WeatherSubsystem.Time)]
    [Range(1, 72)]
    private float hoursInDay = 24;
    public float HoursInDay { get => this.hoursInDay; set => this.hoursInDay = Mathf.Clamp(value, 1, 72); }

    [SerializeField]
    [Readonly]
    private int dayOfYear;
    public int DayOfYear => this.dayOfYear;

    [SerializeField]
    [Readonly]
    private int dayOfWeek;
    public int DayOfWeek => this.dayOfWeek;

    [SerializeField]
    [Readonly]
    private int monthOfYear;
    public int MonthOfYear => this.monthOfYear;

    [SerializeField]
    [Readonly]
    private int hour;
    public int Hour => this.hour;

    [SerializeField]
    [Readonly]
    private int minute;
    public int Minute => this.minute;

    [SerializeField]
    [Readonly]
    private int second;
    public int Second => this.second;

    public string Date => $"{this.hour:00}:{this.minute:00}:{this.second:00}";

    [Header("Sun")]
    public Light Sun;

    public Gradient SunColor = new()
    { 
        colorKeys = new[]
        { 
            new GradientColorKey(new Color(0.0f, 0.0f, 0.0f, 0.0f), 0.0f), 
            new GradientColorKey(new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.5f), 
            new GradientColorKey(new Color(0.0f, 0.0f, 0.0f, 0.0f), 1.0f)
        }
    };

    [SerializeField]
    [Range(0.0f, 5.0f)]
    private float sunIntensityMultiplier = 1.0f;
    public float SunIntensityMultiplier { get => this.sunIntensityMultiplier; set => this.sunIntensityMultiplier = Mathf.Clamp(value, 0.0f, 5.0f); }

    public AnimationCurve SunIntensity = new(
        new Keyframe(0.1f, 0.0f, 4.0f, 4.0f, 0.0f, 0.0f), 
        new Keyframe(0.3f, 1.0f, 0.0f, 0.0f, 0.67f, 0.5f),
        new Keyframe(0.7f, 1.0f, 0.0f, 0.0f, 0.5f, 0.67f),
        new Keyframe(0.9f, 0.0f, -4.0f, -4.0f, 0.0f, 0.0f)
    );

    [Header("Shadow")]

    [SerializeField]
    [Range(-360.0f, 360.0f)]
    private float shadowAngle = -30.0f;
    public float ShadowAngle { get => this.shadowAngle; set => this.shadowAngle = Mathf.Clamp(value, -360.0f, 360.0f); }

    public AnimationCurve ShadowIntensity = new(
        new Keyframe(0.3f, 0.0f, 4.0f, 4.0f, 0.0f, 0.061f), 
        new Keyframe(0.4f, 0.8f, 4.0f, 0.0f, 0.33f, 0.33f),
        new Keyframe(0.5f, 0.8f, 0.0f, 0.0f, 0.0f, 0.0f),
        new Keyframe(0.6f, 0.8f, 0.0f, -4.0f, 0.33f, 0.33f),
        new Keyframe(0.7f, 0.0f, -4.0f, -4.0f, 0.0f, 0.0f)
    );

    [Header("Fog °C")]

    [SerializeField]
    [Range(0.0f, 5.0f)]
    private float fogTemperatureThreshold = 2.5f;
    public float FogTemperatureThreshold { get => this.fogTemperatureThreshold; set => this.fogTemperatureThreshold = Mathf.Clamp(value, 0.0f, 5.0f); }

    [SerializeField]
    [Range(0.0f, 0.1f)]
    private float maxFogDensity = 0.075f;
    public float MaxFogDensity { get => this.maxFogDensity; set => this.maxFogDensity = Mathf.Clamp01(value); }

    public Gradient FogColor = new()
    { 
        colorKeys = new[]
        { 
            new GradientColorKey(new Color(0.0f, 0.0f, 0.0f, 0.0f), 0.0f), 
            new GradientColorKey(new Color(0.6237985f, 0.6645451f, 0.8113208f, 1.0f), 0.3f), 
            new GradientColorKey(new Color(0.66f, 0.88f, 1.0f, 1.0f), 0.5f), 
            new GradientColorKey(new Color(0.6237985f, 0.6645451f, 0.8113208f, 1.0f), 0.7f),
            new GradientColorKey(new Color(0.0f, 0.0f, 0.0f, 0.0f), 1.0f)
        }
    };

    [Header("Temperature °C")]
    
    [SerializeField]
    [DisableIf(nameof(Subsystems), WeatherSubsystem.Temperature)]
    [Range(-100.0f, 100.0f)]
    private float temperature = 15.0f;
    public float Temperature { get => this.temperature; set => this.temperature = Mathf.Clamp(value, -100.0f, 100.0f); }

    [Header("Humidity %")]

    [SerializeField]
    [DisableIf(nameof(Subsystems), WeatherSubsystem.Humidity)]
    [Range(0.0f, 1.0f)]
    private float humidity = 0.4f;
    public float Humidity { get => this.humidity; set => this.humidity = Mathf.Clamp01(value); }

    [Header("Evaporation %")]

    [SerializeField]
    [DisableIf(nameof(Subsystems), WeatherSubsystem.Evaporation)]
    [Range(0.0f, 1.0f)]
    private float evaporation;
    public float Evaporation { get => this.evaporation; }

    [Header("Wind %")]
    
    [SerializeField]
    [DisableIf(nameof(Subsystems), WeatherSubsystem.WindIntensity)]
    [Range(0.0f, 1.0f)]
    private float windIntensity;
    public float WindIntensity { get => this.windIntensity; }

    [SerializeField]
    [DisableIf(nameof(Subsystems), WeatherSubsystem.WindDirection)]
    private Vector3 windDirection;
    public Vector3 WindDirection { get => this.windDirection; set => this.windDirection = value.normalized; }

    private IAudioSource ambientDay;
    private IAudioSource ambientNight;

    private void Start()
    {
        this.UpdatePosition();
        this.UpdateTime();
        this.UpdateDayNight();
        this.UpdateTemperature();
        this.UpdateHumidity();
        this.UpdateFog();
        this.UpdateWind();
        this.UpdateEvaporation();
    }

    private void Update()
    {
        if (this.Enabled == false)
        {
            return;
        }

        if (this.Subsystems.HasFlag(WeatherSubsystem.Time))
        {
            this.timeOfDay += (1.0f / this.dayLengthInSeconds) * Time.deltaTime;
        }

        if (this.timeOfDay > 0.3f && this.timeOfDay < 0.7f)
        {
            if (this.ambientDay == null)
            {
                this.ambientDay = this.Biome?.AmbientDay?.Play();
            }

            if (this.ambientDay != null)
            {
                this.ambientDay.Volume = Mathf.MoveTowards(this.ambientDay.Volume, 1.0f, Time.deltaTime / 1.0f);
            }

            if (this.ambientNight != null)
            {
                this.ambientNight.Volume = Mathf.MoveTowards(this.ambientNight.Volume, 0.0f, Time.deltaTime / 1.0f);
            }
        }
        else
        {
            if (this.ambientNight == null)
            {
                this.ambientNight = this.Biome?.AmbientNight?.Play();
            }

            if (this.ambientDay != null)
            {
                this.ambientDay.Volume = Mathf.MoveTowards(this.ambientDay.Volume, 0.0f, Time.deltaTime / 1.0f);
            }

            if (this.ambientNight != null)
            {
                this.ambientNight.Volume = Mathf.MoveTowards(this.ambientNight.Volume, 1.0f, Time.deltaTime / 1.0f);
            }
        }

        this.UpdatePosition();
        this.UpdateTime();
        this.UpdateDayNight();
        this.UpdateFog();
        this.UpdateHumidity();
        this.UpdateTemperature();
        this.UpdateWind();
        this.UpdateEvaporation();
    }

    public void UpdatePosition()
    {
        if (this.Camera != null)
        {
            this.transform.position = this.Camera.position + this.CameraOffset;
        }
    }

    private void UpdateTime()
    {
        if (this.timeOfDay > 1.0f)
        {
            this.timeOfDay = 0.0f;
            this.timeOfYear += (1.0f / 365.0f);
        }

        if (this.timeOfYear > 1.0f)
        {
            this.timeOfYear = 0.0f;
        }

        this.dayOfYear = Mathf.RoundToInt( this.timeOfYear * this.daysInYear ) + 1;
        this.monthOfYear = Mathf.RoundToInt( this.timeOfYear * this.monthsInYear ) + 1;

        this.dayOfWeek = this.dayOfYear % 7 + 1;

        this.hour = Mathf.RoundToInt( this.timeOfDay * this.hoursInDay );
        this.minute = Mathf.RoundToInt( this.timeOfDay * this.hoursInDay * 60 ) % 60;
        this.second = Mathf.RoundToInt( this.timeOfDay * this.hoursInDay * 60 * 60 ) % 60;
    }

    private void UpdateDayNight()
    {
        RenderSettings.ambientLight = this.AmbientColor.Evaluate(this.timeOfDay);

        if (this.Sun)
        {
            this.Sun.color = this.SunColor.Evaluate(this.timeOfDay);
            this.Sun.transform.eulerAngles = new Vector3( Mathf.Clamp(360.0f * this.timeOfDay - 90.0f, 20.0f, 160.0f), this.shadowAngle, 0.0f);
            this.Sun.intensity = this.SunIntensity.Evaluate(this.timeOfDay) * this.sunIntensityMultiplier;
            this.Sun.shadowStrength = Mathf.Clamp01(this.ShadowIntensity.Evaluate(this.timeOfDay));
        }
    }

    private void UpdateFog()
    {
        RenderSettings.fog = Mathf.Abs( this.temperature ) < this.FogTemperatureThreshold && this.humidity > 0.4;
        RenderSettings.fogDensity = this.MaxFogDensity * this.humidity;
        RenderSettings.fogColor = this.FogColor.Evaluate(this.timeOfDay);
    }

    private void UpdateTemperature()
    {
        if (this.Biome == null)
        {
            return;
        }

        if (this.Subsystems.HasFlag(WeatherSubsystem.Temperature))
        {
            this.temperature = 
                Mathf.Lerp( this.Biome.Temperature.YearlyMinTemperature, this.Biome.Temperature.YearlyMaxTemperature, this.Biome.Temperature.TemperatureCurve.Evaluate(this.timeOfYear) ) + 
                this.Biome.Temperature.DailyTemperatureRange * this.Biome.Temperature.TemperatureCurve.Evaluate(this.timeOfDay);

            this.temperature += ( this.GetNoise(this.Biome.Temperature.TemperatureDeviationFrequency) - 0.5f ) * this.Biome.Temperature.TemperatureDeviationRange;
        }
    }

    private void UpdateHumidity()
    {
        if (this.Subsystems.HasFlag(WeatherSubsystem.Humidity))
        {
            var ratio = (1.0f / this.dayLengthInSeconds);

            this.humidity += this.evaporation * ratio * Time.deltaTime;
            this.humidity = Mathf.Clamp01(this.humidity);
        }
    }

    private void UpdateWind()
    {
        if (this.Biome == null)
        {
            return;
        }

        if (this.Subsystems.HasFlag(WeatherSubsystem.WindIntensity))
        {
            this.windIntensity = this.GetNoise(this.Biome.Wind.IntensityFrequency) * this.Biome.Wind.IntensityMultiplier;
        }

        if (this.Subsystems.HasFlag(WeatherSubsystem.WindDirection))
        {
            var angle = this.GetNoise(this.Biome.Wind.DirectionFrequency) * Mathf.PI * 4.0f;

            this.windDirection = new Vector3( Mathf.Sin(angle), 0.0f, Mathf.Cos(angle) );
        }
    }

    private void UpdateEvaporation()
    {
        if (this.Biome == null)
        {
            return;
        }

        if (this.Subsystems.HasFlag(WeatherSubsystem.Evaporation))
        {
            this.evaporation = 
                Mathf.Max( this.temperature, 0.0f ) / 100.0f * this.Biome.Evaporation.TemperatureEvaporationMultiplier +
                -( this.humidity ) * this.Biome.Evaporation.HumidityEvaporationMultiplier +
                this.windIntensity * this.Biome.Evaporation.WindIntensityEvaporationMultiplier +
                this.Biome.Evaporation.Transpiration;

            this.evaporation = Mathf.Clamp01(this.evaporation) * this.Biome.Evaporation.EvaporationMultiplier;
        }
    }

    private float GetNoise(float frequency)
    {
        return Mathf.PerlinNoise(this.timeOfDay * frequency, this.timeOfYear * frequency);
    }

    private void OnValidate()
    {
        this.UpdatePosition();
        this.UpdateTime();
        this.UpdateDayNight();
        this.UpdateTemperature();
        this.UpdateHumidity();
        this.UpdateFog();
        this.UpdateWind();
        this.UpdateEvaporation();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(this.windIntensity, 1.0f - this.windIntensity, 0.0f);
        DebugExtension.DrawArrow(new Vector3(this.transform.position.x + 5.0f, 0.0f, this.transform.position.z + 5.0f), this.windDirection);
    }
}