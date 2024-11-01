using UnityEngine;

public class DetectionTarget : MonoBehaviour
{
    [SerializeField]
    private bool debug;

    [SerializeField]
    private EventChannelSO eventChannel;

    [SerializeField]
    [Readonly]
    private bool isDetected;
    public bool IsDetected => this.isDetected;

    [SerializeField]
    [Readonly]
    [Range(0.0f, 1.0f)]
    private float detectionMeter;

    private float newDetectionMeter;

    public void SetDetectionState(float sensorDetectionRate)
    {
        this.newDetectionMeter = Mathf.Max(this.newDetectionMeter, sensorDetectionRate);
    }

    private void Update()
    {
        this.detectionMeter = this.newDetectionMeter;
        this.detectionMeter = Mathf.Clamp01(this.detectionMeter);

        this.isDetected = this.detectionMeter >= 1.0f;

        if (this.detectionMeter > 0.0f)
        {
            EventBus.Raise(this, this.eventChannel, new DetectionEventParameters
            {
                Target = this.gameObject,
                DetectionFactor = this.detectionMeter
            });
        }

        this.newDetectionMeter = 0.0f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.Lerp(Color.cyan, Color.red, this.detectionMeter);
        Gizmos.DrawWireSphere(this.transform.position, 0.1f);
    }

    private void OnDrawGizmos()
    {
        if (this.debug)
        {
            this.OnDrawGizmosSelected();
        }
    }
}
