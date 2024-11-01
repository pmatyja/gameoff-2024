using System;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class CharacterController2D : MonoBehaviour
{
    [Header("State")]

    [SerializeField]
    [Readonly]
    private bool isMoving;
    public bool IsMoving => this.isMoving;

    [SerializeField]
    [Readonly]
    private bool isWalking;
    public bool IsWalking => this.isWalking;

    [SerializeField]
    [Readonly]
    private bool isRunning;
    public bool IsRunning => this.isRunning;

    [SerializeField]
    [Readonly]
    private bool isCrouching;
    public bool IsCrouching => this.isCrouching;

    [SerializeField]
    [Readonly]
    private bool isJumping;
    public bool IsJumping => this.isJumping;

    [SerializeField]
    [Readonly]
    private bool hasJumped;
    public bool HasJumped => this.hasJumped;
    
// BEGIN_DIVERGENCE | owen | Use jump input to prevent player from holding jump to bounce, and to make the player fall early when released.
    [SerializeField]
    [Readonly]
    private bool releasedJumpInput;
    public bool ReleasedJumpInput => this.releasedJumpInput;
// END_DIVERGENCE | owen

    [SerializeField]
    [Readonly]
    private bool isGrounded;
    public bool IsGrounded => this.isGrounded;

    [SerializeField]
    [Readonly]
    private bool isFalling;
    public bool IsFalling => this.isFalling;

    [SerializeField]
    [Readonly]
    private Vector3 position;
    public Vector3 Position => this.position;

    [SerializeField]
    [Readonly]
    private Vector2 desiredMotion;
    public Vector2 DesiredMotion => this.desiredMotion;

    [SerializeField]
    [Readonly]
    private Vector2 facingDirection = Vector2.right;
    public Vector2 FacingDirection => this.facingDirection;

    [SerializeField]
    [Readonly]
    [Range(0.0f, 20.0f)]
    private float maxSpeed;
    public float MaxSpeed => this.maxSpeed;

    [SerializeField]
    [Readonly]
    private Vector2 velocity;
    public Vector2 Velocity => this.velocity;

    [SerializeField]
    [Readonly]
    private float linearVelocity;
    public float LinearVelocity => this.linearVelocity;

    [Header("Properties")]

    [SerializeField]
    [Range(0.0f, 2f)]
    private float minMoveDistance = 0.001f;
    public float MinMoveDistance => this.minMoveDistance;

    [SerializeField]
    [Range(0.0f, 10f)]
    private float minMoveVelocity = 0.001f;
    public float MinMoveVelocity => this.minMoveVelocity;

    [SerializeField]
    [Range(0.0f, 20.0f)]
    private float crouchingSpeed = 3f;
    public float CrouchingSpeed => this.crouchingSpeed;

    [SerializeField]
    [Range(0.0f, 20.0f)]
    private float walkingSpeed = 5f;
    public float WalkingSpeed => this.walkingSpeed;

    [SerializeField]
    [Range(0.0f, 20.0f)]
    private float runningSpeed = 7f;
    public float RunningSpeed => this.runningSpeed;

    [SerializeField]
    [Range(0.0f, 11.4f)]
    private float animationReferenceSpeed = 5.7f;
    public float AnimationReferenceSpeed => this.animationReferenceSpeed;

    [Header("Ground")]

    [SerializeField]
    private LayerMask groundMask;
    public LayerMask GroundMask => this.groundMask;

    [SerializeField]
    private LayerMask platformMask;
    public LayerMask PlatformMask => this.platformMask;

    [SerializeField]
    [Range(0.0f, 0.5f)]
    private float groundCastLength = 0.1f;
    public float GroundCastLength => this.groundCastLength;

    [SerializeField]
    [Readonly]
    private Transform ground;
    public Transform Ground => this.ground;

    [Readonly]
    private Vector2 groundNormal;
    public Vector2 GroundNormal => this.groundNormal;

    [SerializeField]
    [Readonly]
    private float groundAngle;
    public float GroundAngle => this.groundAngle;

    private float groundCastRadius => this.CapsuleCollider.size.x * 0.5f * 0.98f;
    private Vector2 groundCastOrigin => new Vector2
    (
        this.transform.position.x, 
        this.transform.position.y + this.CapsuleCollider.offset.y - this.CapsuleCollider.size.y / 2 + this.CapsuleCollider.size.x / 2
    );

    [SerializeField]
    [Readonly]
    private PhysicsMaterial2D groundMaterial;
    public PhysicsMaterial2D GroundMaterial => this.groundMaterial;

    [Header("Physics")]

    [SerializeField]
    [Range(0.1f, 50.0f)]
    private float maxHorizontalVelocity = 5.0f;
    public float MaxHorizontalVelocity => this.maxHorizontalVelocity;

    [SerializeField]
    [Range(0.1f, 50.0f)]
    private float maxVerticalVelocity = 10.0f;
    public float MaxVerticalVelocity => this.maxVerticalVelocity;

    [SerializeField]
    [Range(0.1f, 50.0f)]
    private float jumpForce = 5.0f;
    public float JumpForce => this.jumpForce;

    [SerializeField]
    [Range(1.0f, 3.0f)]
    private float fallingGravityMultiplier = 2.0f;
    public float FallingGravityMultiplier => this.fallingGravityMultiplier;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float fallingForce = 0.2f;
    public float FallingForce => this.fallingForce;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float fallingTransitionDuration = 0.35f;
    public float FallingTransitionDuration => this.fallingTransitionDuration;

    protected Animator Animator;
    protected Rigidbody2D PhysicsBody;
    protected CapsuleCollider2D CapsuleCollider;

    private Transform defaultParent;
    private Vector3 previousPosition;
    private float initialGravityScale;

    public event Action Jumped;

    public void Crouch(bool state)
    {
        this.isCrouching = this.isGrounded && state;
    }

    public void Run(bool state)
    {
        this.isRunning = this.isGrounded && state;
    }

    public void Move(Vector2 motion)
    {
        this.desiredMotion = motion;
    }

    public void AddForce(Vector2 force)
    {
        this.PhysicsBody.AddForce(force);
    }

    public void VelocityChange(Vector2 force)
    {
        this.PhysicsBody.velocity = force;
    }

    public void Jump(bool isPressed)
    {
        if (isPressed == false)
        {
// BEGIN_DIVERGENCE | owen | Don't allow the player to jump again until the jump button has been released
            this.releasedJumpInput = true;
// END_DIVERGENCE | owen
            return;
        }

        if (this.IsJumping)
        {
            return;
        }

        if (this.hasJumped)
        {
            return;
        }
// BEGIN_DIVERGENCE | owen | Don't allow the player to jump again until the jump button has been released
        if (this.IsGrounded && this.releasedJumpInput)
        {
            this.releasedJumpInput = false;
            
            this.Jumped?.Invoke();
// END_DIVERGENCE | owen
            
            this.PhysicsBody.velocity = new Vector2
            (
                this.PhysicsBody.velocity.x,
                PhysicsInterpolation.GetJumpForceWeight(this.jumpForce)
            );

            this.hasJumped = true;
        }
    }

    protected virtual void OnEnable()
    {
        this.Animator = this.GetComponentInChildren<Animator>();

        this.TryGetComponent<Rigidbody2D>(out this.PhysicsBody);
        this.TryGetComponent<CapsuleCollider2D>(out this.CapsuleCollider);

        this.facingDirection = new Vector2(1.0f, 0.0f);
        this.defaultParent = this.transform.parent;
        this.initialGravityScale = this.PhysicsBody.gravityScale;
    }

    protected virtual void FixedUpdate()
    {
        // IsMoving

        this.previousPosition   = this.position;
        this.position           = this.transform.position;
        this.linearVelocity     = this.velocity.magnitude;
        this.isMoving           = this.linearVelocity > this.minMoveVelocity;

        if (Mathf.Abs(this.velocity.x) > 0.01f)
        {
            this.facingDirection = new Vector2(this.velocity.x, 0.0f).normalized;
        }

        // Ground check

        var groundInfo = Physics2D.CircleCast
        (
            this.groundCastOrigin, 
            this.groundCastRadius, 
            Vector2.down, 
            this.groundCastLength, 
            this.groundMask
        );

        this.isGrounded = groundInfo.collider != default;

        // IsGrounded

        if (this.isGrounded)
        {
            this.isJumping = false;
            this.hasJumped = false;
            this.isFalling = false;

            this.ground = groundInfo.transform;
            this.groundNormal = groundInfo.normal;
            this.groundAngle = Vector3.Angle(groundInfo.normal, Vector3.up);
            this.groundMaterial = groundInfo.collider?.sharedMaterial;

            // Platform

            if (this.platformMask.ContainsLayer(this.ground.gameObject.layer))
            {
                this.transform.SetParent(groundInfo.transform);
            }
            else
            {
                this.transform.SetParent(this.defaultParent);
            }
        }
        else
        {
// BEGIN_DIVERGENCE | owen | make player start falling faster if jump button was release before apex
            if (this.releasedJumpInput)
            {
                if (this.velocity.y > 0.0f)
                {
                    this.PhysicsBody.velocity = new Vector2
                    (
                        this.PhysicsBody.velocity.x,
                        this.PhysicsBody.velocity.y - this.fallingForce * Mathf.Pow(Physics2D.gravity.y, 2) * Time.fixedDeltaTime
                    );
                }
            }
// END_DIVERGENCE | owen 
            
            this.isCrouching = false;
            this.isJumping = this.velocity.y >= 0.0f;
            this.isFalling = this.velocity.y < 0.0f;

            this.ground = null;
            this.groundNormal = Vector3.up;
            this.groundAngle = 0.0f;
            this.groundMaterial = null;

            this.transform.SetParent(this.defaultParent);
        }

        // Movement

        if (this.isCrouching)
        {
            this.maxSpeed = this.crouchingSpeed;
        }
        else if (this.isRunning)
        {
            this.maxSpeed = this.runningSpeed;
        }
        else
        {
            this.maxSpeed = this.walkingSpeed;
        }

        this.PhysicsBody.AddForce(new Vector2(this.desiredMotion.x * this.maxSpeed, 0.0f));

        // Extra gravity for falling

        if (this.isFalling)
        {
            this.PhysicsBody.gravityScale = this.initialGravityScale * this.fallingGravityMultiplier;
        }
        else
        {
            this.PhysicsBody.gravityScale = this.initialGravityScale;
        }

        // Velocity limits

        this.PhysicsBody.velocity = new Vector2
        (
            Mathf.Clamp(this.PhysicsBody.velocity.x, -this.maxHorizontalVelocity, this.maxHorizontalVelocity),
            Mathf.Clamp(this.PhysicsBody.velocity.y, -this.maxVerticalVelocity, this.maxVerticalVelocity)
        );

        this.velocity = this.PhysicsBody.velocity;

        // Apply animation properties

        if (this.Animator != null)
        {
            this.Animator.SetBool(AnimatorParameters.IsMoving, this.IsMoving);
            this.Animator.SetBool(AnimatorParameters.IsCrouching, this.IsCrouching);
            this.Animator.SetBool(AnimatorParameters.IsWalking, this.IsWalking);
            this.Animator.SetBool(AnimatorParameters.IsRunning, this.IsRunning);
            this.Animator.SetBool(AnimatorParameters.IsJumping, this.IsJumping);
            this.Animator.SetBool(AnimatorParameters.HasJumped, this.HasJumped);
            this.Animator.SetBool(AnimatorParameters.IsGrounded, this.IsGrounded);
            this.Animator.SetBool(AnimatorParameters.IsFalling, this.IsFalling);

            this.Animator.SetFloat(AnimatorParameters.LinearVelocity, this.linearVelocity);

            this.Animator.SetFloat(AnimatorParameters.VelocityX, Mathf.Abs(this.velocity.x));
            this.Animator.SetFloat(AnimatorParameters.VelocityY, Mathf.Abs(this.velocity.y));
        }

        // Reset motion

        //this.desiredMotion = Vector3.zero;
    }

    protected void OnDrawGizmosSelected()
    {
        var groundInfo = Physics2D.CircleCast
        (
            this.groundCastOrigin,
            this.groundCastRadius,
            Vector2.down,
            this.groundCastLength,
            this.groundMask
        );

        if (groundInfo.collider != default)
        {
            this.isGrounded = groundInfo.distance <= this.groundCastLength;

            Gizmos.DrawWireSphere
            (
                new Vector3(groundInfo.point.x, groundInfo.point.y, 0.0f),
                this.groundCastRadius
            );
        }
        else
        {
            Gizmos.DrawWireSphere
            (
                this.groundCastOrigin - new Vector2(0.0f, this.groundCastLength),
                this.groundCastRadius
            );
        }
    }

    protected virtual void OnValidate()
    {
        this.Animator = this.GetComponentInChildren<Animator>();

        this.TryGetComponent<Rigidbody2D>(out this.PhysicsBody);
        this.TryGetComponent<CapsuleCollider2D>(out this.CapsuleCollider);

        this.facingDirection = new Vector2(1.0f, 0.0f);
    }
}