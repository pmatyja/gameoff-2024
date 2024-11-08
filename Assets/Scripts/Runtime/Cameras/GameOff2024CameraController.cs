using System;
using OCSFX.Utility;
using OCSFX.Utility.Attributes;
using Unity.Cinemachine;
using UnityEngine;

namespace Runtime.Cameras
{
    public class GameOff2024CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineInputAxisController _inputAxisController;
        [SerializeField] private CinemachineCamera _cinemachineCamera;
        [SerializeField, MinMaxRange(1, 10)] private Vector2 _zoomRange;
        [SerializeField, Range(0,2)] private float _zoomSmoothTime = 0.2f;
        
        private float _zoomVelocity;
        private float _targetZoomValue;
        
        [Header("Debug")]
        [SerializeField] private bool _showDebug;

        private void Awake()
        {
            if (!ResolveDependencies()) return;
            _inputAxisController.enabled = false;
            _targetZoomValue = _cinemachineCamera.Lens.OrthographicSize;
        }

        private void OnEnable()
        {
            InputHandler.Get().OnDragCameraInput += OnDragCameraInput;
            InputHandler.Get().OnZoomInput += OnZoomInput;
        }

        private void OnDisable()
        {
            InputHandler.Get().OnDragCameraInput -= OnDragCameraInput;
            InputHandler.Get().OnZoomInput -= OnZoomInput;
        }
        
        private void OnDragCameraInput(bool pressed)
        {
            _inputAxisController.enabled = pressed;
        }
        
        private void OnZoomInput(float delta)
        {
            _targetZoomValue = Mathf.Clamp(_targetZoomValue - delta, _zoomRange.x, _zoomRange.y);
        }

        private void UpdateZoom()
        {
            var currentSize = _cinemachineCamera.Lens.OrthographicSize;
            _cinemachineCamera.Lens.OrthographicSize = Mathf.SmoothDamp(currentSize, _targetZoomValue, ref _zoomVelocity, _zoomSmoothTime, Mathf.Infinity, Time.deltaTime);
        }

        private void Update()
        {
            UpdateZoom();
        }

        private bool ResolveDependencies()
        {
            return gameObject.TryResolveComponent(ref _inputAxisController)
                   && gameObject.TryResolveComponent(ref _cinemachineCamera);
        }
    }
}
