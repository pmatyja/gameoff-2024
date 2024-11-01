using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public abstract class WeatherEffect : MonoBehaviour
{
    public abstract bool IsActive { get; }

    [SerializeField]
    [Range(1, 10000)]
    protected int ParticleCount = 100;

    [Header("Emission")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float emissionWeight;
    public float EmissionWeight { get => this.emissionWeight; set => this.emissionWeight = Mathf.Clamp01(value); }

    [SerializeField]
    [Range(0.0001f, 360.0f)]
    protected float AmbientDuration = 10.0f;

    [SerializeField]
    [Range(0.0001f, 1.0f)]
    protected float AmbientSmoothness = 0.0002f;
    
    [Header("Wind")]
    public bool ApplyWind;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float windWeight;
    public float WindWeight { get => this.windWeight; set => this.windWeight = Mathf.Clamp01(value); }

    [SerializeField]
    private Vector3 maxWindVelocity;
    public Vector3 MaxWindVelocity { get => this.maxWindVelocity; set => this.maxWindVelocity = value; }

    [SerializeField]
    [Range(0.0f, 360.0f)]
    public float Angle;

    public bool IsLocalEffect;
    public AmbientSO ambient;

    protected WeatherSystem WeatherSystem;
    protected ParticleSystem ParticleSystem;

    private ParticleSystem.EmissionModule emission;
    private ParticleSystem.MinMaxCurve count;
    private ParticleSystem.ForceOverLifetimeModule force;

    [SerializeField]
    private IAudioSource audioSource;

    protected abstract void UpdateEffect();

    private void Start()
    {
        this.WeatherSystem = this.GetComponentInParent<WeatherSystem>();
        this.ParticleSystem = this.GetComponentInChildren<ParticleSystem>();

        this.emission = this.ParticleSystem.emission;
        this.count = this.emission.rateOverTime;
        this.force = this.ParticleSystem.forceOverLifetime;

        this.audioSource = this.ambient?.Play(this.IsLocalEffect ? this.gameObject : null);

        if (this.audioSource != null)
        {
            this.audioSource.Volume = 0.0f;
        }
    }

    private void Update()
    {
        this.UpdateEffect();

        if (this.IsActive)
        {
            this.EmissionWeight = Mathf.MoveTowards(this.EmissionWeight, 1.0f, Time.deltaTime / this.AmbientDuration);

            if (this.audioSource != null)
            {
                this.audioSource.Volume = Mathf.MoveTowards(this.audioSource.Volume, 1.0f, this.AmbientSmoothness);
            }
        }
        else
        {
            this.EmissionWeight = Mathf.MoveTowards(this.EmissionWeight, 0.0f, Time.deltaTime / this.AmbientDuration);

            if (this.audioSource != null)
            {
                this.audioSource.Volume = Mathf.MoveTowards(this.audioSource.Volume, 0.0f, this.AmbientSmoothness);
            }
        }

        this.emission.rateOverTime = this.count = Mathf.Lerp(0.0f, this.ParticleCount, this.EmissionWeight);
        
        if (this.ApplyWind)
        {
            this.force.x = this.WeatherSystem.WindDirection.x * this.MaxWindVelocity.x * this.WeatherSystem.WindIntensity;
            this.force.y = this.WeatherSystem.WindDirection.y * this.MaxWindVelocity.y * this.WeatherSystem.WindIntensity;
            this.force.z = this.WeatherSystem.WindDirection.z * this.MaxWindVelocity.z * this.WeatherSystem.WindIntensity;
        }
    }
}
