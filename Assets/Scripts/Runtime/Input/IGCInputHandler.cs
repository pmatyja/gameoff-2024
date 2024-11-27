using System;
using OCSFX.Generics;
using UnityEngine;

namespace Runtime.Input
{
    public class IGCInputHandler : SingletonScriptableObject<IGCInputHandler>, IInputHandler
    {
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        protected static void EditorInit()
        {
            UnityEditor.EditorApplication.delayCall += () => Get();
        }
#endif

        public void SetActive(bool active) => IsActive = active;

        public bool IsActive { get; private set; }

        public event Action<Vector2> OnGameplayMoveInput;
        public event Action<bool> OnGameplayDragCameraInput;
        public event Action<float> OnGameplayCameraZoomInput;
        public event Action OnGameplayInteractInput;
        
        public void RaiseMoveInput(Vector2 input)
        {
            if (!IsActive) return;
            OnGameplayMoveInput?.Invoke(input);
        }
        
        public void RaiseDragCameraInput(bool isDragging)
        {
            if (!IsActive) return;
            OnGameplayDragCameraInput?.Invoke(isDragging);
        }
        
        public void RaiseCameraZoomInput(float zoom)
        {
            if (!IsActive) return;
            OnGameplayCameraZoomInput?.Invoke(zoom);
        }

        public void RaiseInteractInput()
        {
            if (!IsActive) return;
            OnGameplayInteractInput?.Invoke();
        }
    }
}