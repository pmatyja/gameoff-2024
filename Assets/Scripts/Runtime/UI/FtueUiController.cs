using System;
using Runtime.Interactions;
using UnityEngine;

namespace Runtime.UI
{
    public class FtueUiController : OCSFX.Generics.Singleton<FtueUiController>
    {
        private int _usedHoverCount;

        public static event Action OnMouseHoverClickable;
        
        private bool _isHovered;
        private Camera _mainCamera;
        
        private void OnEnable()
        {
            _mainCamera = Camera.main;
        }
        
        private void Update()
        {
            if (_usedHoverCount >= GameOff2024GameSettings.Get().FtueClickHintCount)
            {
                enabled = false;
            }
            
            var currentHoverStatus = Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out var hit, 100f);
            
            if (currentHoverStatus == _isHovered) return;
            
            _isHovered = currentHoverStatus;

            if (!_isHovered) return;
            
            if (!hit.collider.TryGetComponent(out PointerInteractable pointerInteractable)) return;
            
            var pointerInteractableParent =
                pointerInteractable.GetComponentInParent<PointerInteractableParent>();

            if (!pointerInteractableParent) return;
            
            _usedHoverCount++;
            OnMouseHoverClickable?.Invoke();
        }
    }
}