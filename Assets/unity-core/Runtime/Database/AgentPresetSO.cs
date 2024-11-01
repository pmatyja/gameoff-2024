using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = nameof(AgentPresetSO), menuName = "Lavgine/Database/Agent Preset")]
public class AgentPresetSO : ScriptableObject
{
    [Header("Properties")]

    [SerializeField]
    [Range(0.0f, 0.1f)]
    private float minMoveDistance = 0.001f;
    public float MinMoveDistance => this.minMoveDistance;

    [Header("Ground")]

    [SerializeField]
    private LayerMask groundMask;
    public LayerMask GroundMask => this.groundMask;

    [SerializeField]
    [Range(0.0f, 0.5f)]
    private float groundCastLength = 0.1f;
    public float GroundCastLength => this.groundCastLength;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float minFallDistance = 1.0f;
    public float MinFallDistance => this.minFallDistance;

    [Header("Moving Platform")]

    [SerializeField]
    private LayerMask platformMask;
    public LayerMask PlatformMask => this.platformMask;

    [Header("Movement")]

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float acceleration = 0.15f;
    public float Acceleration => this.acceleration;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float deceleration = 0.15f;
    public float Deceleration => this.deceleration;

    [SerializeField]
    private bool holdToWalk;
    public bool HoldToWalk => this.holdToWalk;

    [SerializeField]
    [Range(0.0f, 2.5f)]
    private float crouchingSpeed = 1.2f;
    public float CrouchingSpeed => this.crouchingSpeed;

    [SerializeField]
    [Range(0.0f, 3.6f)]
    private float walkingSpeed = 1.8f;
    public float WalkingSpeed => this.walkingSpeed;

    [SerializeField]
    [Range(0.0f, 11.4f)]
    private float runningSpeed = 5.7f;
    public float RunningSpeed => this.runningSpeed;

    [SerializeField]
    [Range(0.0f, 11.4f)]
    private float animationReferenceSpeed = 5.7f;
    public float AnimationReferenceSpeed => this.animationReferenceSpeed;
    
    [Header("Jump")]

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float airControlFactor = 0.2f;
    public float AirControlFactor => this.airControlFactor;

    [SerializeField]
    [Range(0.0f, 2.0f)]
    private float jumpHeight = 1.6f;
    public float JumpHeight => this.jumpHeight;

    [SerializeField]
    [Range(-1.0f, 1.0f)]
    private float jumpHeightOffset = 0.5f;
    public float JumpHeightOffset => this.jumpHeightOffset;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float fallingTransitionDuration = 0.35f;
    public float FallingTransitionDuration => this.fallingTransitionDuration;

    [Header("Physics")]

    [FormerlySerializedAs("maxVelocity")]
    [SerializeField]
    [Range(0.1f, 50.0f)]
    private float maxLinearVelocity = 10.0f;
    public float MaxLinearVelocity => this.maxLinearVelocity;
}