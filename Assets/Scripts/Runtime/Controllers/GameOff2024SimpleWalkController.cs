using Runtime.Utility;
using UnityEngine;

namespace Runtime.Controllers
{
    public class GameOff2024SimpleWalkController: GameOff2024ControllerBase
    {
        // Modified version of CMF SimpleWalkController

        [SerializeField] private GameOff2024Mover _mover;
        [SerializeField] private FallProbe _fallProbe;

        [field: Space]
        [field: SerializeField] public float MovementSpeed { get; set; } = 7f;
        [field: SerializeField] public float JumpSpeed { get; set; } = 10f;
        [field: SerializeField] public float GravityScale { get; set; } = 1f;
        [field: SerializeField] public float TurnSpeed { get; set; } = 10f;
        [field: SerializeField] public float Acceleration { get; set; } = 10f;
        
        private const float _MAX_ROT_DELTA_FACTOR = 1080f;

        private Transform _cameraTransform;
        
        private float _currentVerticalSpeed;
        private bool _isGrounded;
        private Vector3 _currentVelocity;
        private Vector3 _moveDirection;
        
        private Transform _transform;

        private Vector2 _movementInput;
        private bool _jumpInput;

        private void Awake()
        {
            _transform = transform;

            if (!_mover)
            {
                if (!TryGetComponent(out _mover))
                {
                    Debug.LogWarning($"{nameof(GameOff2024SimpleWalkController)} requires a {nameof(GameOff2024Mover)} component to function. Adding one now.");
                    _mover = gameObject.AddComponent<GameOff2024Mover>();
                }   
            }

            _cameraTransform = GameOff2024Statics.GetMainCamera().transform;
        }

        public void SetMovementDirection(Vector2 direction) => _movementInput = direction;

        private void FixedUpdate()
        {
            HandleGroundChecks();
            
            //Add player movement to velocity;
            var targetVelocity = GameOff2024Statics.GetCameraRelativeMoveDirection(_movementInput, _cameraTransform) * MovementSpeed;
            var isReversing = Vector3.Dot(_moveDirection.normalized, targetVelocity.normalized) < 0f;
            var accelerationFactor = isReversing ? 2 * Acceleration : Acceleration;
            
            _moveDirection = Vector3.MoveTowards(_moveDirection, targetVelocity, accelerationFactor * Time.deltaTime);
            
            //Calculate velocity;
            var velocity = _moveDirection;
            
            //Use the fall probe to check whether the player should be allowed to walk off a ledge;
            if (_fallProbe && !_fallProbe.IsSafeFall)
            {
                if (_fallProbe.TryGetSafeAlternativePosition(out var safeAlternativePosition))
                {
                    var safeAlternativeDirection = (safeAlternativePosition - _transform.position).normalized;
                    velocity = Vector3.Lerp(_currentVelocity, safeAlternativeDirection * (_moveDirection.magnitude),
                        accelerationFactor * Time.deltaTime);
                }
                else
                {
                    velocity = Vector3.zero;
                }
            }
            
            RotateToFaceMoveDirection();
            HandleGravity();
            HandleJumping();
            
            // Add vertical velocity;
            velocity += _transform.up * _currentVerticalSpeed;
            
            //Store velocity for later use;
            _currentVelocity = velocity;
            
            _mover.SetExtendSensorRange(_isGrounded);
            _mover.SetVelocity(velocity);
        }

        private void HandleGroundChecks()
        {
            //Run initial mover ground check;
            _mover.CheckForGround();
            
            //If character was not grounded in the last frame and is now grounded, call 'OnGroundContactRegained' function;
            if (!_isGrounded && _mover.IsGrounded)
            {
                OnGroundContactRegained(_currentVelocity);
            }
            
            //Check whether the character is grounded and store result;
            _isGrounded = _mover.IsGrounded;
        }

        private void HandleGravity()
        {
            if (!_isGrounded)
            {
                _currentVerticalSpeed += Physics.gravity.y * GravityScale * Time.deltaTime;
            }
            else
            {
                if (_currentVerticalSpeed <= 0f)
                {
                    _currentVerticalSpeed = 0f;
                }
            }
        }

        private void HandleJumping()
        {
            if (_jumpInput && _isGrounded)
            {
                OnJumpStart();
                _currentVerticalSpeed = JumpSpeed;
                _isGrounded = false;
            }
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
            return TurnSpeed * _MAX_ROT_DELTA_FACTOR;
        }
        
        //This function is called when the controller has landed on a surface after being in the air;
        void OnGroundContactRegained(Vector3 _collisionVelocity)
        {
            //Call 'OnLand' delegate function;
            OnLand?.Invoke(_collisionVelocity);
        }
        
        //This function is called when the controller has started a jump;
        void OnJumpStart()
        {
            //Call 'OnJump' delegate function;
            OnJump?.Invoke(_currentVelocity);
        }

        public override Vector3 GetVelocity() => _currentVelocity;

        public override Vector3 GetMovementVelocity() => _currentVelocity;

        public override bool IsGrounded() => _isGrounded;
    }
}