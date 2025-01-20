using System;
using OCSFX.Attributes;
using Runtime.Interactions;
using UnityEngine;

namespace Runtime.UI
{
    public class HintManager : OCSFX.Generics.Singleton<HintManager>
    {
        public static event Action OnHoverClickableEvent;
        public static event Action OnRotateCameraInputEvent;
        public static event Action OnZoomCameraInputEvent;
        
        public static event Action<bool> OnGameOff2024CameraStatusChangedEvent;
        
        private Camera _mainCamera;
        
        [Header("Debug")]
        [SerializeField, ReadOnly] private bool _isGameOff2024CameraActive;
        [SerializeField, ReadOnly] private bool _isHovered;
        [SerializeField, ReadOnly] private bool _hasClicked;
        [SerializeField, ReadOnly] private bool _hasZoomed;
        [SerializeField, ReadOnly] private bool _hasRotated;
        
        private void OnEnable()
        {
            _mainCamera = Camera.main;
            
            GameOff2024CameraEventsHandler.OnGameOff2024CameraStatusChangedEvent += OnGameOff2024CameraStatusChanged;
            
            var inputHandler = InputHandler.Get();
            if (!inputHandler) return;
            
            inputHandler.OnGameplayDragCameraInput += OnRotateCameraInput;
            inputHandler.OnGameplayCameraZoomInput += OnZoomCameraInput;
        }

        private void OnDisable()
        {
            GameOff2024CameraEventsHandler.OnGameOff2024CameraStatusChangedEvent -= OnGameOff2024CameraStatusChanged;
            
            var inputHandler = InputHandler.Get();
            if (!inputHandler) return;
            
            inputHandler.OnGameplayDragCameraInput -= OnRotateCameraInput;
            inputHandler.OnGameplayCameraZoomInput -= OnZoomCameraInput;
        }

        private void OnGameOff2024CameraStatusChanged(bool isActive)
        {
            if (_isGameOff2024CameraActive == isActive) return;
            
            _isGameOff2024CameraActive = isActive;
            
            OnGameOff2024CameraStatusChangedEvent?.Invoke(isActive);
        }

        private void Update()
        {
            if (_hasClicked) return;
            
            var didHit = Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out var hit, 100f);
            if (!didHit || !IsValidHoveredObject(hit))
            {
                _isHovered = false;
                return;
            }
            
            _hasClicked = DidClick();
            
            // Gate to prevent update spam
            if (_isHovered) return;
            
            _isHovered = true;
            OnHoverClickableEvent?.Invoke();
        }
        
        private void OnRotateCameraInput(bool status)
        {
            if (_isGameOff2024CameraActive)
            {
                _hasRotated = true;
                return;
            }
            
            OnRotateCameraInputEvent?.Invoke();
        }
        
        private void OnZoomCameraInput(float value)
        {
            if (_isGameOff2024CameraActive)
            {
                _hasZoomed = true;
                return;
            }
            
            OnZoomCameraInputEvent?.Invoke();
        }

        private bool DidClick()
        {
            // TODO: Remove use of old input system
            return Input.GetMouseButtonDown(0);
        }
        
        private bool HasZoomedAndRotated()
        {
            return _hasZoomed && _hasRotated;
        }
        
        private bool HasCompletedAllActions()
        {
            return _hasClicked && _hasZoomed && _hasRotated;
        }
        
        private bool IsValidHoveredObject(RaycastHit hit)
        {
            return 
                hit.collider.TryGetComponent(out PointerInteractable pointerInteractable) &&
                pointerInteractable.GetComponentInParent<PointerInteractableParent>();
        }
    }
}