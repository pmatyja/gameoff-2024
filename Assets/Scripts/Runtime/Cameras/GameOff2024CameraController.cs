using OCSFX.Utility.Debug;
using Unity.Cinemachine;
using UnityEngine;

namespace Runtime.Cameras
{
    public class GameOff2024CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineInputAxisController _inputAxisController;
        
        [Header("Debug")]
        [SerializeField] private bool _showDebug;

        private void Awake()
        {
            if (!_inputAxisController)
            {
                if (!TryGetComponent(out _inputAxisController))
                {
                    OCSFXLogger.LogError($"No {nameof(CinemachineInputAxisController)} found on " + name, this, _showDebug);
                }

                return;
            }
            
            _inputAxisController.enabled = false;
        }

        private void OnEnable()
        {
            InputHandler.Get().OnDragCameraInput += OnDragCameraInput;
        }

        private void OnDisable()
        {
            InputHandler.Get().OnDragCameraInput -= OnDragCameraInput;
        }
        
        private void OnDragCameraInput(bool pressed)
        {
            _inputAxisController.enabled = pressed;
        }
    }
}
