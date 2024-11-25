using System;
using System.Collections.Generic;
using OCSFX.Attributes;
using OCSFX.Utility;
using OCSFX.Utility.Attributes;
using Runtime;
using Runtime.Controllers;
using Runtime.Utility;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class GameOff2024PlayerCharacterAvatarController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private GameOff2024SimpleWalkController _walkController;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [FormerlySerializedAs("_castShadowMode")] [SerializeField] private ShadowCastingMode _shadowCastingMode = ShadowCastingMode.On;
    [Header("Animator Parameters")]
    [SerializeField] private AnimatorParamRef _moveSpeedParamRef = new AnimatorParamRef("MoveSpeed", AnimatorControllerParameterType.Float);
    [SerializeField] private AnimatorParamRef _isFacingCameraParamRef = new AnimatorParamRef("IsFacingCamera", AnimatorControllerParameterType.Bool);
    
    [Header("Settings")]
    [Tooltip("The dead zone for the facing camera check." +
             "\nIf the dot product of the forward vector of the character and the camera is within this range, " +
             "no changes are made to the facing camera parameter.")]
    [SerializeField, MinMaxRange(-1,1)] private Vector2 _facingCameraDeadZone = new Vector2(-.25f, .25f); 
    [SerializeField, MinMaxRange(-90, 90)] private Vector2 _verticalRotationLimits = new Vector2(-90, 90);
    [SerializeField] private bool _faceCameraWhenStill = true;
    
    private Vector2 _movementInput = Vector2.zero;
    
    [field: Header("Getters")]
    [field: SerializeField, ReadOnly] public bool IsFacingCamera { get; private set; } = true;
    [field: SerializeField, ReadOnly] public float NormalizedLocomotionSpeed { get; private set; }
    [field: SerializeField, ReadOnly, Range(-1,1)] public float FacingCameraDot { get; private set; } 

    private void OnEnable()
    {
        InputHandler.Get().OnGameplayMoveInput += OnGameplayMoveInput;
        
        _spriteRenderer.shadowCastingMode = _shadowCastingMode;
    }

    private void OnDisable()
    {
        InputHandler.Get().OnGameplayMoveInput -= OnGameplayMoveInput;
    }

    private void Update()
    {
        HandleBillboard();
        
        if (!_walkController) return;

        AnimateLocomotion();
        HandleCameraFaceDirection();
    }
    
    private float GetNormalizedLocomotionSpeed()
    {
        var maxSpeed = _walkController.MovementSpeed;
        var currentHorizontalSpeed = new Vector2(_walkController.GetVelocity().x, _walkController.GetVelocity().z).sqrMagnitude;
        
        // map the current speed to the range [0, 1]
        return currentHorizontalSpeed.Map01(0, maxSpeed);
    }

    private void AnimateLocomotion()
    { 
        var shouldAnimate = _movementInput.sqrMagnitude > 0 && _walkController.GetVelocity().sqrMagnitude > 0;
        
        NormalizedLocomotionSpeed = shouldAnimate ? GetNormalizedLocomotionSpeed() : 0;
        
        _moveSpeedParamRef.SetValue(NormalizedLocomotionSpeed);
    }
    
    private void HandleBillboard()
    {
        var cam = GameOff2024Statics.GetMainCamera();
        
        if (!cam) return;

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
            _isFacingCameraParamRef.SetValue(IsFacingCamera);
            return;
        }
        
        if (_faceCameraWhenStill && _walkController.GetVelocity().sqrMagnitude < 0.01f)
        {
            IsFacingCamera = true;
            _isFacingCameraParamRef.SetValue(IsFacingCamera);
            return;
        }
        
        var cam = GameOff2024Statics.GetMainCamera();
        
        if (!cam) return;
        
        var camForward = cam.transform.forward.normalized;
    
        FacingCameraDot = Vector3.Dot(GameOff2024Statics.GetCameraRelativeMoveDirection(_movementInput), camForward);
        
        var isInDeadZone = FacingCameraDot > _facingCameraDeadZone.x && FacingCameraDot < _facingCameraDeadZone.y;
        
        if (!isInDeadZone)
        {
            IsFacingCamera = FacingCameraDot < 0;
        }
        
        _isFacingCameraParamRef.SetValue(IsFacingCamera);
    }
    
    private void OnGameplayMoveInput(Vector2 movement)
    {
        _movementInput = movement;
        
        if (Mathf.Abs(_movementInput.x) > 0.01f)
        {
            _spriteRenderer.flipX = _movementInput.x > 0;
        }
    }

    private void OnValidate()
    {
        if (_spriteRenderer)
        {
            _spriteRenderer.shadowCastingMode = _shadowCastingMode;
        }
    }
}
