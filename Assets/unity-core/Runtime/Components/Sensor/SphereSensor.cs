using System.Collections.Generic;
using UnityEngine;

public class SphereSensor : MonoBehaviour
{
    [SerializeField]
    private EventChannelSO eventChannel;

    [SerializeField]
    [Readonly]
    private bool detected;
    public bool Detected => this.detected;

    [SerializeField]
    [Readonly]
    [Range(0f, 1f)]
    private float detectionMetric;
    public float DetectionMetric => this.detectionMetric;

    [SerializeField]
    private Vector3 offset;
    public Vector3 Offset => this.offset;

    [SerializeField]
    private float radius = 1.0f;
    public float Radius => this.radius;

    [SerializeField]
    private LayerMask layer;

    [SerializeField]
    [Range(1, 10000)]
    private int minNumberOfObjects = 1;
    public int MinNumberOfObjects => this.minNumberOfObjects;

    [SerializeField]
    [Readonly]
    private int objectsCount;
    public int ObjectsCount => this.objectsCount;

    [SerializeField]
    [Readonly]
    private Collider[] objects;
    public IReadOnlyCollection<Collider> Objects => this.objects;

    [Header("Debug")]

    [SerializeField]
    private bool debugEnabled;

    private void Update()
    {
        if (this.objects == null)
        {
            this.objects = new Collider[this.minNumberOfObjects];
        }

        this.objectsCount = Physics.OverlapSphereNonAlloc(this.transform.position + this.offset, this.radius, this.objects, this.layer.value);
        this.detectionMetric = this.objectsCount / (float)this.minNumberOfObjects;

        if (this.objectsCount < this.minNumberOfObjects)
        {
            this.detected = false;
            return;
        }

        this.detected = true;

        EventBus.Raise(this, this.eventChannel, new CollidersEventParameters
        {
            Source = this.gameObject,
            Colliders = this.objects
        });
    }

    private void OnValidate()
    {
        this.objects = new Collider[this.minNumberOfObjects];
    }

    private void OnDrawGizmos()
    {
        if (this.debugEnabled)
        {
            this.OnDrawGizmosSelected();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.Lerp(Color.yellow, Color.red, this.detectionMetric);
        Gizmos.DrawWireSphere(this.transform.position + this.offset, this.radius);
    }
}
