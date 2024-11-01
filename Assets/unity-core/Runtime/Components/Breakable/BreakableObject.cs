using System.Collections;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [SerializeField]
    private EventChannelSO channel;

    [SerializeField]
    private bool debug = true;

    [SerializeField]
    private GameObject replacement;

    [Header("Properties")]

    [SerializeField]
    public bool ExplicitBreakOnly;

    [SerializeField]
    [Range(0.0f, 5.0f)]
    private float delay;

    [SerializeField]
    [Range(0.1f, 30.0f)]
    private float destoryFractureDelay = 3.0f;

    [SerializeField]
    private Vector3 breakDirection;

    [Header("Trigger")]

    [SerializeField]
    private LayerMask triggeredBy;

    [SerializeField]
    private Bounds triggerArea = new(Vector3.zero, Vector3.one * 0.5f);

    [Header("Lanuch pad")]

    [SerializeField]
    private bool isLaunchPad;

    [SerializeField]
    [Range(1.0f, 5.0f)]
    private float pushForce = 3.0f;

    [SerializeField]
    private Vector3 pushDirection = Vector3.one;

    [Header("Fracture particles")]

    [SerializeField]
    [Range(1.0f, 3000.0f)]
    private float explosionForce = 2.0f;

    [SerializeField]
    [Range(1.0f, 10.0f)]
    private float explosionRadius = 2.0f;

    [SerializeField]
    private ForceMode explosionForceMode;

    [Header("Chain Trigger")]

    [SerializeField]
    private bool isChainTrigger;

    [SerializeField]
    [Range(1.0f, 20.0f)]
    private float chainTriggerRadius = 3.0f;

    [Header("Camera Shake")]

    [SerializeField]
    private bool shakeCamera;

    [SerializeField]
    private CameraShake CameraShake = new();

    private bool isBroken;

    private static Collider[] Colliders = new Collider[16];

    [ContextMenu("Break")]
    public void Break()
    {
        if (this.isBroken)
        {
            return;
        }

        this.StartCoroutine(this.OnBreak());
    }

    private void OnEnable()
    {
        EventBus.AddListener<BreakObjectEventParameters>(this.OnBreakEvent, this.channel);
    }

    private void OnDisable()
    {
        EventBus.AddListener<BreakObjectEventParameters>(this.OnBreakEvent, this.channel);
    }

    private void Update()
    {
        var count = Physics.OverlapBoxNonAlloc(this.transform.position + this.triggerArea.center, this.triggerArea.size * 0.5f, Colliders, this.transform.rotation, this.triggeredBy.value, QueryTriggerInteraction.Ignore);

        for (var i = 0; i < count; i++)
        {
            var other = Colliders[i];

            if (this.isLaunchPad)
            {
                if (other.TryGetComponent<CharacterController2D>(out var obj))
                {
                    obj.VelocityChange(new Vector3(obj.Velocity.x, 0.0f) + this.pushDirection.normalized * PhysicsInterpolation.GetJumpForceWeight(this.pushForce));
                }
            }

            this.Break();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (this.isChainTrigger)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(this.transform.position, this.chainTriggerRadius);
        }

        if (this.breakDirection == Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position, this.explosionRadius);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(this.transform.position, this.transform.position + this.breakDirection);
            Gizmos.DrawWireSphere(this.transform.position + this.breakDirection, this.explosionRadius);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(this.transform.position + this.triggerArea.center, this.triggerArea.size);
    }

    private void OnDrawGizmos()
    {
        if (this.debug)
        {
            this.OnDrawGizmosSelected();
        }
    }

    private void OnBreakEvent(object sender, BreakObjectEventParameters parameters)
    {
        this.Break();
    }

    private IEnumerator OnBreak()
    {
        if (this.isBroken)
        {
            yield break;
        }

        this.isBroken = true;

        if (this.delay > 0.0f)
        {
            yield return Wait.Seconds(this.delay);
        }

        if (this.shakeCamera)
        {
            EventBus.Raise(this, new ShakeCameraTargetEventParameters
            {
                WorldSpace = true,
                Position = this.transform.position,
                Magnitude = this.CameraShake.Magnitude,
                Duration = this.CameraShake.Duration
            });
        }

        var instance = UnityEngine.GameObject.Instantiate(this.replacement, this.transform.position, this.transform.rotation);
        var rigidBodies = instance.GetComponentsInChildren<Rigidbody>();

        foreach (var body in rigidBodies)
        {
            if (this.breakDirection == Vector3.zero)
                body.AddExplosionForce(this.explosionForce, this.transform.position + Random.insideUnitSphere, this.explosionRadius);
            else
                body.AddRelativeForce(this.explosionForce * this.breakDirection, this.explosionForceMode);

            if (body.gameObject.TryGetComponent<FractureComponent>(out var fracture))
            {
                fracture.Animate(this.destoryFractureDelay);
            }
            else
            {
                body.gameObject.AddComponent<FractureComponent>().Animate(this.destoryFractureDelay);
            }
        }

        if (this.isChainTrigger)
        {
            var colliders = Physics.OverlapSphere(this.transform.position, this.chainTriggerRadius);

            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent<BreakableObject>(out var breakableObject))
                {
                    if (breakableObject.ExplicitBreakOnly == false)
                    {
                        breakableObject.Break();
                    }
                }
            }
        }

        UnityEngine.GameObject.Destroy(this.gameObject);
    }
}
