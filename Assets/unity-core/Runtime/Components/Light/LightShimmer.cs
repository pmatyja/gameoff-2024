using UnityEngine;

public class LightShimmer : MonoBehaviour
{
    [SerializeField]
    private Light target;

    [SerializeField]
    [Readonly]
    private float timeOffset;

    [SerializeField]
    [Range(0.00001f, 10f)]
    private float frequency = 0.01f;

    [SerializeField]
    [Range(0.01f, 1000f)]
    private float intensity = 0.8f;

    [SerializeField]
    [Range(0.01f, 1000f)]
    private float shimmerRange = 1.0f;

    public void Awake()
    {
        this.timeOffset = Random.value * short.MaxValue;
    }

    public void Update()
    {
        if (this.target == null)
        {
            return;
        }

        this.target.intensity = this.intensity + Mathf.PerlinNoise1D(this.timeOffset) * this.shimmerRange;
        this.timeOffset += Time.deltaTime * this.frequency;
    }
}
