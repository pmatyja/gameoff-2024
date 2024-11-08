using OCSFX.Utility;
using OCSFX.Utility.Attributes;
using Unity.Cinemachine;
using UnityEngine;

namespace Runtime.Cameras
{
    public abstract class GameOff2024CameraControllerBase : MonoBehaviour
    {
        [SerializeField] protected CinemachineInputAxisController _inputAxisController;
        [SerializeField] protected CinemachineCamera _cinemachineCamera;
        [SerializeField, Range(0,2)] protected float _zoomSmoothTime = 0.2f;

        protected abstract Vector2 GetZoomRange();
        
        [Header("Debug")]
        [SerializeField] private bool _showDebug;
        
        protected abstract LensSettings.OverrideModes _lensMode { get; }
        
        protected float _zoomVelocity;
        protected float _targetZoomValue;

        protected virtual void Awake()
        {
            ResolveDependencies();
            InitializeCameraZoom();
            
            _inputAxisController.enabled = false;
        }

        protected virtual void OnEnable()
        {
            InputHandler.Get().OnDragCameraInput += OnDragCameraInput;
            InputHandler.Get().OnZoomInput += OnZoomInput;
            
            _inputAxisController.enabled = false;
        }

        protected virtual void OnDisable()
        {
            InputHandler.Get().OnDragCameraInput -= OnDragCameraInput;
            InputHandler.Get().OnZoomInput -= OnZoomInput;
            
            _inputAxisController.enabled = false;
        }
        
        protected virtual void OnDragCameraInput(bool pressed)
        {
            _inputAxisController.enabled = pressed;
        }

        protected virtual void OnZoomInput(float delta)
        {
            var _zoomRange = GetZoomRange();
            _targetZoomValue = Mathf.Clamp(_targetZoomValue - delta, _zoomRange.x, _zoomRange.y);
        }
        
        protected void InitializeCameraZoom()
        {
            _targetZoomValue = _lensMode switch
            {
                LensSettings.OverrideModes.Orthographic => _cinemachineCamera.Lens.OrthographicSize,
                LensSettings.OverrideModes.Perspective => _cinemachineCamera.Lens.FieldOfView,
                _ => _targetZoomValue
            };
        }

        protected abstract void UpdateZoom();

        protected virtual void Update()
        {
            UpdateZoom();
        }

        protected virtual bool ResolveDependencies()
        {
            var validComponentRefs = gameObject.TryResolveComponent(ref _inputAxisController)
                   && gameObject.TryResolveComponent(ref _cinemachineCamera);

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