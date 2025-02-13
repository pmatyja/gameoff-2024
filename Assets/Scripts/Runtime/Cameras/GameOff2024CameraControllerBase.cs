using OCSFX.EZFMOD.Utility;
using Unity.Cinemachine;
using UnityEngine;

namespace Runtime.Cameras
{
    [RequireComponent(typeof(CinemachineCamera))]
    [RequireComponent(typeof(CinemachineOrbitalFollow))]
    [RequireComponent(typeof(CinemachineInputAxisController))]
    [RequireComponent(typeof(CinemachineRotationComposer))]
    [DisallowMultipleComponent]
    public abstract class GameOff2024CameraControllerBase : MonoBehaviour
    {
        protected CinemachineInputAxisController _inputAxisController;
        protected CinemachineCamera _cinemachineCamera;
        protected CinemachineOrbitalFollow _orbitalFollow;
        protected CinemachineRotationComposer _rotationComposer;
        
        [SerializeField, Range(0.1f, 10)] private float _zoomDeltaMultiplier = 1f;
        [SerializeField, Range(0,2)] protected float _zoomSmoothTime = 0.2f;

        protected abstract Vector2 GetZoomRange();

        public CinemachineCamera GetCinemachineCamera() => _cinemachineCamera;
        
        [Header("Debug")]
        [SerializeField] private bool _showDebug;
        
        protected abstract LensSettings.OverrideModes _lensMode { get; }
        
        protected float _zoomVelocity;
        protected float _targetZoomValue;

        protected virtual void Awake()
        {
            ResolveDependencies();
            InitializeZoom();
            
            _inputAxisController.enabled = false;
        }

        protected virtual void OnEnable()
        {
            GameOff2024CameraEventsHandler.OnCameraDeactivatedEvent += OnGameOff2024CameraDeactivated;
            
            InputHandler.Get().OnGameplayDragCameraInput += OnGameplayDragCameraInput;
            InputHandler.Get().OnGameplayCameraZoomInput += OnGameplayCameraZoomInput;
            
            _inputAxisController.enabled = false;
        }

        protected virtual void OnDisable()
        {
            GameOff2024CameraEventsHandler.OnCameraDeactivatedEvent -= OnGameOff2024CameraDeactivated;
            
            InputHandler.Get().OnGameplayDragCameraInput -= OnGameplayDragCameraInput;
            InputHandler.Get().OnGameplayCameraZoomInput -= OnGameplayCameraZoomInput;
            
            _inputAxisController.enabled = false;
        }

        protected virtual void OnGameplayDragCameraInput(bool pressed)
        {
            if (!GameOff2024CameraEventsHandler.IsGameOff2024CameraActive) return;
            
            _inputAxisController.enabled = pressed;
        }

        protected virtual void OnGameplayCameraZoomInput(float delta)
        {
            if (!GameOff2024CameraEventsHandler.IsGameOff2024CameraActive) return;
            
            var _zoomRange = GetZoomRange();
            
            delta *= _zoomDeltaMultiplier;
            
            _targetZoomValue = Mathf.Clamp(_targetZoomValue - delta, _zoomRange.x, _zoomRange.y);
        }
        
        protected virtual void OnGameOff2024CameraDeactivated(ICinemachineMixer cinemachineMixer, ICinemachineCamera cinemachineCamera)
        {
            _targetZoomValue = GetZoomRange().y;
            
            _inputAxisController.enabled = false;
        }

        protected abstract void InitializeZoom();

        protected abstract void UpdateZoom();

        protected virtual void Update()
        {
            UpdateZoom();
        }

        protected virtual bool ResolveDependencies()
        {
            var validComponentRefs 
                = gameObject.TryResolveComponent(ref _inputAxisController)
                && gameObject.TryResolveComponent(ref _cinemachineCamera)
                && gameObject.TryResolveComponent(ref _orbitalFollow)
                && gameObject.TryResolveComponent(ref _rotationComposer);

            if (!validComponentRefs) return false;

            _cinemachineCamera.Lens.ModeOverride = _lensMode;
            
            return true;
        }
        
        protected virtual void OnValidate()
        {
            if (!ResolveDependencies()) return;
            
            _cinemachineCamera.Lens.ModeOverride = _lensMode;
        }
    }
}