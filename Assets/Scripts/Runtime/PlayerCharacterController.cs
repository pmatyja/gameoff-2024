using System;
using OCSFX.Utility.Debug;
using Runtime;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _turnSpeed = 1f;
    [SerializeField] private float _gravityScale = 1f;
    [SerializeField] private GroundCheck _groundCheck;
    [SerializeField] private FallProbe _fallProbe;
    
    private CharacterController _characterController;
    private InputHandler _inputHandler;
    private Vector2 _moveInputDirection;
    private Vector3 _moveDirection;
    private float _fallTime;

    private const float _IMPLICIT_ROTATION_SPEED = 1080f;
    
    [Header("Debug")]
    [SerializeField] private bool _showDebug;
    [SerializeField, Readonly] private bool _isGrounded;
    
    private void Awake()
    {
        if (!_characterController) _characterController = GetComponent<CharacterController>();
        
        if (!_characterController)
        {
            OCSFXLogger.LogError($"{nameof(CharacterController)} not found on {nameof(PlayerCharacterController)} GameObject ({name})", this, _showDebug);
        }

        _inputHandler = InputHandler.Get();
    }

    private void OnEnable()
    {
        _inputHandler.OnMoveInput += OnMoveInput;
    }
    
    private void OnDisable()
    {
        _inputHandler.OnMoveInput -= OnMoveInput;
    }

    private void Update()
    {
        HandleGravity();
        RotateToFaceMoveDirection();
        Move();
    }
    
    private static Camera _mainCamera;
    
    private static Camera GetMainCamera()
    {
        if (!_mainCamera) _mainCamera = Camera.main;

        return _mainCamera;
    }
    
    private Vector3 GetCameraRelativeMoveDirection(Vector2 inputDirection)
    {
        var cam = GetMainCamera();
        if (!cam) return Vector3.zero;
        
        var forward = cam.transform.forward;
        var right = cam.transform.right;
        
        forward.y = 0f;
        right.y = 0f;
        
        forward.Normalize();
        right.Normalize();
        
        return forward * inputDirection.y + right * inputDirection.x;
    }

    private void Move()
    {
        if (_moveInputDirection == Vector2.zero)
        {
            _moveDirection = Vector3.zero;
            return;
        }
        
        _moveDirection = GetCameraRelativeMoveDirection(_moveInputDirection);
        
        var moveDelta = _moveDirection * (_moveSpeed * Time.deltaTime);

        if (_fallProbe && _fallProbe.IsFallZone) return;
        
        _characterController.Move(moveDelta);
    }
    
    private void HandleGravity()
    {
        _isGrounded = IsGrounded();
        
        if (_isGrounded)
        {
            // We need to check that the character controller is also grounded
            if (_characterController.isGrounded)
            {
                _fallTime = 0f;
                return;
            }
        }
        
        _fallTime += Time.deltaTime;

        var gravity = Physics.gravity * _gravityScale;
        var fallDelta = gravity * (_fallTime * Time.deltaTime);
        
        _characterController.Move(fallDelta);
    }
    
    private bool IsGrounded()
    {
        return _groundCheck ? _groundCheck.IsGrounded : _characterController.isGrounded;
    }
    
    private void RotateToFaceMoveDirection()
    {
        if (_moveDirection == Vector3.zero)
        {
            return;
        }
        
        var targetRotation = Quaternion.LookRotation(_moveDirection);
        var rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, GetMaxRotationDelta() * Time.deltaTime);
        
        transform.rotation = rotation;
    }

    private float GetMaxRotationDelta()
    {
        return _turnSpeed * _IMPLICIT_ROTATION_SPEED;
    }

    private void OnMoveInput(Vector2 inputDirection)
    {
        _moveInputDirection = inputDirection;
    }
}
