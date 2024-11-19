using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BlockMoverScript : MonoBehaviour
{
    [SerializeField]
    [Range(1f, 20f)]
    private float speedPerUnit = 10f;

    [SerializeField]
    [Range(1, 10)]
    private int distance = 3;

    [SerializeField]
    private BlockDirection direction = BlockDirection.PositiveY;
    [SerializeField]
    private BlockDirectionScope directionScope = BlockDirectionScope.WorldSpace;
    
    [Header("Events")]
    [field: SerializeField] public UnityEvent OnBlockMoveBeginEvent { get; private set; }
    [field: SerializeField] public UnityEvent OnBlockMoveEndEvent { get; private set; }

    [Header("Debug")]
    [SerializeField]
    private bool isDebugEnabled;

    [SerializeField]
    private bool showIndividualBlocks = true;

    [SerializeField]
    private bool manualTrigger;

    private Coroutine coroutine;
    private Vector3 offset;
    private Vector3 pointA;
    private Vector3 pointB;

    private readonly WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    public void Activate()
    {
        if (this.coroutine != null)
        {
            return;
        }

        this.coroutine = this.StartCoroutine(this.LateFixedUpdate());
    }

    private void Start()
    {
        this.pointA = this.transform.position;
        this.pointB = this.GetDestination();
    }

    private void OnDrawGizmos()
    {
        if (this.isDebugEnabled == false)
        {
            return;
        }

        Debug.DrawLine(this.pointA, this.pointB, Color.yellow);

        var boundsA = new Bounds();
        var boundsB = new Bounds();

        foreach (Transform child in this.transform)
        {
            if (child.gameObject.activeInHierarchy == false)
            {
                continue;
            }

            var localBounds = new Bounds();

            if (child.TryGetComponent<BoxCollider>(out var boxCollider))
            {
                localBounds = boxCollider.bounds;
            }
            else if (child.TryGetComponent<MeshCollider>(out var meshCollider))
            {
                localBounds = meshCollider.bounds;
            }

            var localOffset = 
                Vector3.zero 
                + child.transform.localPosition
                - child.transform.position
                + localBounds.center
            ;

            boundsA.Encapsulate(new Bounds(this.pointA + localOffset, localBounds.size));
            boundsB.Encapsulate(new Bounds(this.pointB + localOffset, localBounds.size));

            if (this.showIndividualBlocks)
            {
                DebugExtension.DrawBounds(new Bounds(this.pointA + localOffset, localBounds.size), Color.cyan);
                DebugExtension.DrawBounds(new Bounds(this.pointB + localOffset, localBounds.size), Color.green);
            }
        }

        if (this.showIndividualBlocks == false)
        {
            var bounds = this.gameObject.getColliderBounds();
            var destination = GetWorldDirection(this.direction) * this.distance;

            DebugExtension.DrawBounds(new Bounds(bounds.center, bounds.size), Color.cyan);
            DebugExtension.DrawBounds(new Bounds(bounds.center + destination, bounds.size), Color.green);
        }
    }

    private void OnValidate()
    {
        if (Application.isPlaying == false)
        {
            this.pointA = this.transform.position;
            this.pointB = this.GetDestination();
            return;
        }

        if (this.manualTrigger == false)
        {
            return;
        }

        this.Activate();
        this.manualTrigger = false;
    }

    private IEnumerator LateFixedUpdate()
    {
        yield return waitForFixedUpdate;
        
        var destination = this.GetDestination();
        
        this.OnMoveBegin();
        
        while (this.isActiveAndEnabled)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, destination, this.speedPerUnit * Time.deltaTime);
            this.offset = this.pointA - this.transform.position;

            if (Vector3.Distance(this.transform.position, destination) < 0.01f)
            {
                this.transform.position = destination;
                break;
            }
            
            yield return waitForFixedUpdate;
        }
        
        this.OnMoveEnd();

        this.direction = NegateDirection(this.direction);
        this.coroutine = null;
    }

    private void OnMoveBegin()
    {
        this.OnBlockMoveBeginEvent?.Invoke();
        EventBus.Raise<OnBlockMoveEventParameters>(this, 
            new OnBlockMoveEventParameters(this.gameObject, this.direction, this.transform.position));
    }
    
    private void OnMoveEnd()
    {
        this.OnBlockMoveEndEvent?.Invoke();
        EventBus.Raise<OnBlockMoveFinishedEventParameters>(this, 
            new OnBlockMoveFinishedEventParameters(this.gameObject, this.direction, transform.position));
    }

    private Vector3 GetDestination()
    {
        switch (directionScope)
        {
            default:
            case BlockDirectionScope.WorldSpace:
                return this.transform.position + GetWorldDirection(this.direction) * this.distance;
            case BlockDirectionScope.LocalSpace:
                return this.transform.position + GetLocalDirection(this.transform, this.direction) * this.distance;
        }
    }

    private static Vector3 GetWorldDirection(BlockDirection direction)
    {
        switch (direction)
        {
            case BlockDirection.PositiveX:  return Vector3.right;
            case BlockDirection.NegativeX:  return Vector3.left;
            case BlockDirection.PositiveY:  return Vector3.up;
            case BlockDirection.NegativeY:  return Vector3.down;
            case BlockDirection.PositiveZ:  return Vector3.forward;
            case BlockDirection.NegativeZ:  return Vector3.back;
        }

        return Vector3.zero;
    }

    private static Vector3 GetLocalDirection(Transform targetTransform, BlockDirection direction)
    {
        switch (direction)
        {
            case BlockDirection.PositiveX: return targetTransform.right;
            case BlockDirection.NegativeX: return -targetTransform.right;
            case BlockDirection.PositiveY: return targetTransform.up;
            case BlockDirection.NegativeY: return -targetTransform.up;
            case BlockDirection.PositiveZ: return targetTransform.forward;
            case BlockDirection.NegativeZ: return -targetTransform.forward;
        }
        
        return Vector3.zero;
    }

    private static BlockDirection NegateDirection(BlockDirection direction)
    {
        switch (direction)
        {
            case BlockDirection.PositiveX:  return BlockDirection.NegativeX;
            case BlockDirection.NegativeX:  return BlockDirection.PositiveX;
            case BlockDirection.PositiveY:  return BlockDirection.NegativeY;
            case BlockDirection.NegativeY:  return BlockDirection.PositiveY;
            case BlockDirection.PositiveZ:  return BlockDirection.NegativeZ;
            case BlockDirection.NegativeZ:  return BlockDirection.PositiveZ;
        }

        return direction;
    }

    public enum BlockDirectionScope
    {
        WorldSpace,
        LocalSpace
    }
}