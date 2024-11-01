using UnityEngine;

public class MovementStateBehaviour : MonoBehaviour
{
    [SerializeField]
    private EventChannelSO eventChannel;

    [SerializeField]
    [Range(0.0f, 0.01f)]
    private float minDistance = 0.0001f;

    [SerializeField]
    [Readonly]
    private MovementState state;
    public MovementState State => this.state;

    [SerializeField]
    [Readonly]
    private Vector3 velocity;
    public Vector3 Velocity => this.velocity;

    [SerializeField]
    [Readonly]
    private Vector3 direction;
    public Vector3 Direction => this.direction;

    private Vector3 lastPosition;

    private void OnEnable()
    {
        this.lastPosition = this.transform.position;
    }

    private void FixedUpdate()
    {
        this.velocity = (this.transform.position - this.lastPosition);
        this.direction = this.velocity.normalized;

        var distance = Vector3.Distance(this.transform.position, this.lastPosition);
        if (distance > this.minDistance)
        {
            if (this.state == MovementState.Starting)
            {
                this.RaiseEvent(MovementState.Moving);
            }

            if (this.state == MovementState.Idle)
            {
                this.RaiseEvent(MovementState.Starting);
            }
        }
        else
        {
            if (this.state == MovementState.Stoping)
            {
                this.RaiseEvent(MovementState.Idle);
            }

            if (this.state == MovementState.Starting || this.state == MovementState.Moving)
            {
                this.RaiseEvent(MovementState.Stoping);
            }
        }

        this.lastPosition = this.transform.position;
    }

    private void RaiseEvent(MovementState status)
    {
        this.state = status;

        EventBus.Raise(this, this.eventChannel, new MovementStateEventParameters
        {
            Source = this
        });
    }
}
