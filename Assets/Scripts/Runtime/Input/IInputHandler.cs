using System;
using UnityEngine;

namespace Runtime.Input
{
    public interface IInputHandler
    {
        public void SetActive(bool active);
        public bool IsActive { get; }
        
        public event Action<Vector2> OnGameplayMoveInput;
        public event Action<bool> OnGameplayDragCameraInput;
        public event Action<float> OnGameplayCameraZoomInput;
        public event Action OnGameplayInteractInput;
    }
}