using System.Collections.Generic;
using UnityEngine;

public class BoxSensor : MonoBehaviour
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
    private Quaternion rotation = Quaternion.identity;
    public Quaternion Rotation => this.rotation;

    [SerializeField]
    private Vector3 bounds = Vector3.one;
    public Vector3 Bounds => this.bounds;

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

        this.objectsCount = Physics.OverlapBoxNonAlloc(this.transform.position + this.offset, this.bounds * 0.5f, this.objects, this.rotation, this.layer.value, QueryTriggerInteraction.Ignore);
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
        DebugExtension.UsingTransform(this.transform.position, this.rotation, Vector3.one, () => Gizmos.DrawWireCube(this.offset, this.bounds * 0.5f));
    }
}