using OCSFX.Attributes;
using OCSFX.Utility;
using OCSFX.Utility.Attributes;
using Runtime;
using Runtime.Controllers;
using UnityEngine;

public class GameOff2024PlayerCharacterAvatarController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator _animator;
    [SerializeField] private GameOff2024SimpleWalkController _walkController;
    [Space]
    [SerializeField] private string _moveSpeedParameter = "MoveSpeed";
    [SerializeField] private string _facingCameraParameter = "IsFacingCamera";
    
    [Header("Settings")]
    [Tooltip("The dead zone for the facing camera check." +
             "\nIf the dot product of the forward vector of the character and the camera is within this range, " +
             "no changes are made to the facing camera parameter.")]
    [SerializeField, MinMaxRange(-1,1)] private Vector2 _facingCameraDeadZone = new Vector2(-.25f, .25f); 
    [SerializeField, MinMaxRange(-90, 90)] private Vector2 _verticalRotationLimits = new Vector2(-90, 90);
    [SerializeField] private bool _faceCameraWhenStill = true;

    private int _moveSpeedHash;
    private int _faceCameraHash;
    
    private Vector2 _movementInput = Vector2.zero;
    
    [field: Header("Getters")]
    [field: SerializeField, ReadOnly] public bool IsFacingCamera { get; private set; } = true;
    [field: SerializeField, ReadOnly] public float NormalizedLocomotionSpeed { get; private set; }
    [field: SerializeField, ReadOnly, Range(-1,1)] public float FacingCameraDot { get; private set; } 

    private void OnEnable()
    {
        CacheParameterHashes();
        
        InputHandler.Get().OnMoveInput += OnMoveInput;
    }

    private void OnDisable()
    {
        InputHandler.Get().OnMoveInput -= OnMoveInput;
    }

    private void Update()
    {
        HandleBillboard();
        
        if (!_animator || !_walkController)
        {
            return;
        }

        GetNormalizedLocomotionSpeed();

        if (AnimateLocomotion())
        {
            _animator.SetFloat(_moveSpeedHash, NormalizedLocomotionSpeed);
        }
        else
        {
            NormalizedLocomotionSpeed = 0;
            _animator.SetFloat(_moveSpeedHash, 0);
        }
        
        HandleCameraFaceDirection();
    }
    
    private float GetNormalizedLocomotionSpeed()
    {
        var maxSpeed = _walkController.MovementSpeed;
        var currentHorizontalSpeed = new Vector2(_walkController.GetVelocity().x, _walkController.GetVelocity().z).sqrMagnitude;
        
        // map the current speed to the range [0, 1]
        NormalizedLocomotionSpeed = currentHorizontalSpeed.Map01(0, maxSpeed);
        
        return NormalizedLocomotionSpeed;
    }

    private bool AnimateLocomotion()
    {
        return _movementInput.sqrMagnitude > 0 && _walkController.GetVelocity().sqrMagnitude > 0;
    }
    
    private void HandleBillboard()
    {
        var cam = GameOff2024Statics.GetMainCamera();

        // Calculate the direction from the camera
        var directionFromCamera = transform.position - cam.transform.position;
        var horizontalDirection = new Vector3(directionFromCamera.x, 0, directionFromCamera.z).normalized;
        var verticalAngle = Vector3.SignedAngle(horizontalDirection, directionFromCamera, transform.right);

        // Clamp the vertical angle
        verticalAngle = Mathf.Clamp(verticalAngle, _verticalRotationLimits.x, _verticalRotationLimits.y);
        var clampedDirection = Quaternion.AngleAxis(verticalAngle, transform.right) * horizontalDirection;

        // Set the rotation to look at the clamped direction
        transform.rotation = Quaternion.LookRotation(clampedDirection, Vector3.up);
    }
    
    private void HandleCameraFaceDirection()
    {
        if (!_walkController)
        {
            IsFacingCamera = true;
            _animator.SetBool(_faceCameraHash, IsFacingCamera);
            return;
        }
        
        if (_faceCameraWhenStill && _walkController.GetVelocity().sqrMagnitude < 0.01f)
        {
            IsFacingCamera = true;
            _animator.SetBool(_faceCameraHash, IsFacingCamera);
            return;
        }
        
        var cam = GameOff2024Statics.GetMainCamera();
        var camForward = cam.transform.forward.normalized;
    
        FacingCameraDot = Vector3.Dot(GameOff2024Statics.GetCameraRelativeMoveDirection(_movementInput), camForward);
        
        var isInDeadZone = FacingCameraDot > _facingCameraDeadZone.x && FacingCameraDot < _facingCameraDeadZone.y;
        
        if (!isInDeadZone)
        {
            IsFacingCamera = FacingCameraDot < 0;
        }
        
        if (!_animator)
        {
            return;
        }

        _animator.SetBool(_faceCameraHash, IsFacingCamera);
    }
    
    private void OnMoveInput(Vector2 movement) => _movementInput = movement;
    
    private void CacheParameterHashes()
    {
        _moveSpeedHash = Animator.StringToHash(_moveSpeedParameter);
        _faceCameraHash = Animator.StringToHash(_facingCameraParameter);
    }

    private void OnValidate()
    {
        CacheParameterHashes();
    }
}
