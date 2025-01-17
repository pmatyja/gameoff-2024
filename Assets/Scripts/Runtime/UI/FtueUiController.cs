using System;
using Runtime.Interactions;
using UnityEngine;

namespace Runtime.UI
{
    public class FtueUiController : OCSFX.Generics.Singleton<FtueUiController>
    {
        public static event Action OnMouseHoverClickable;
        
        private bool _isHovered;
        private Camera _mainCamera;
        private bool _hasClicked;
        
        private void OnEnable()
        {
            _mainCamera = Camera.main;
        }

        private void OnDisable()
        {
            
        }

        private void Update()
        {
            var didHit = Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out var hit, 100f);
            if (!didHit)
            {
                _isHovered = false;
                return;
            }

            if (_isHovered)
            {
                if (_hasClicked)
                {
                    enabled = false;
                    return;
                }
                
                // TODO: Remove use of old input system
                _hasClicked = Input.GetMouseButtonDown(0);
                
                return;
            }
            
            if (!hit.collider.TryGetComponent(out PointerInteractable pointerInteractable))
            {
                _isHovered = false;
                return;
            }
            
            var pointerInteractableParent =
                pointerInteractable.GetComponentInParent<PointerInteractableParent>();

            if (!pointerInteractableParent)
            {
                _isHovered = false;
                return;
            }
            
            _isHovered = true;
            OnMouseHoverClickable?.Invoke();
        }
    }
}