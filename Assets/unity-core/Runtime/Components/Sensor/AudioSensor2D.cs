using System;
using UnityEngine;

public class AudioSensor2D : MonoBehaviour
{
    [field: SerializeField]
    public bool Detected { get; private set; }

    [field: SerializeField]
    public Vector3 DetectedPosition { get; private set; }

    [SerializeField]
    [Range(0f, 1f)]
    [Readonly]
    private float detectionState;
    public float DetectionState => this.detectionState;

    [Header("Properties")]

    [SerializeField]
    private EventChannelSO eventChannel;

    [SerializeField]
    [Range(0.01f, 20.0f)]
    private float range;

    [SerializeField]
    private bool debug;

    public void OnEnable()
    {
        EventBus.AddListener<NoiseEventParameters>(this.OnNoiseEvent);
    }

    public void OnDisable()
    {
        EventBus.RemoveListener<NoiseEventParameters>(this.OnNoiseEvent);
    }

    private void OnNoiseEvent(object sender, NoiseEventParameters parameters)
    {
        var distance = Vector3.Distance(this.transform.position, parameters.Position);
        if (distance <= this.range)
        {
            this.Detected = true;
            this.DetectedPosition = parameters.Position;
            this.detectionState = Mathf.Clamp01(distance / this.range);

            EventBus.Raise(this.gameObject, eventChannel, new NoiseDetectedEventParameters
            {
                Source = this.gameObject,
                NoisePosition = parameters.Position
            });
        }
    }

    private void OnDrawGizmosSelected()
    {
        DebugExtension.DrawWireSphere(this.transform.position, this.range, Color.cyan);
    }

    private void OnDrawGizmos()
    {
        if (this.debug)
            this.OnDrawGizmosSelected();
    }
}
