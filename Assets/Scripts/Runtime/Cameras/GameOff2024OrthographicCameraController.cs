using OCSFX.Utility.Attributes;
using Unity.Cinemachine;
using UnityEngine;

namespace Runtime.Cameras
{
    public class GameOff2024OrthographicCameraController : GameOff2024CameraControllerBase
    {
        [Header("Orthographic Settings")]
        [SerializeField, MinMaxRange(1,10)] private Vector2 _zoomRange = new Vector2(2, 6);

        private GameOff2024CameraControllerBase _gameOff2024CameraControllerBaseImplementation;

        protected override Vector2 GetZoomRange() => _zoomRange;
        protected override LensSettings.OverrideModes _lensMode => LensSettings.OverrideModes.Orthographic;

        protected override void InitializeZoom()
        {
            _targetZoomValue = _cinemachineCamera.Lens.OrthographicSize;
        }

        protected override void UpdateZoom()
        {
            var currentSize = _cinemachineCamera.Lens.OrthographicSize;
            _cinemachineCamera.Lens.OrthographicSize = Mathf.SmoothDamp(currentSize, _targetZoomValue, 
                ref _zoomVelocity, _zoomSmoothTime, Mathf.Infinity, Time.deltaTime);
        }
    }
}
