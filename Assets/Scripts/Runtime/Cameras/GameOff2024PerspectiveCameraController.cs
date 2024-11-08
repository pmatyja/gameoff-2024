using OCSFX.Utility.Attributes;
using Unity.Cinemachine;
using UnityEngine;

namespace Runtime.Cameras
{
    public class GameOff2024PerspectiveCameraController : GameOff2024CameraControllerBase
    {
        [Header("Perspective Settings")]
        [SerializeField] private float _fov = 60;
        [SerializeField, MinMaxRange(1,10)] private Vector2 _zoomRange = new Vector2(2, 6);

        protected override Vector2 GetZoomRange() => _zoomRange;
        protected override LensSettings.OverrideModes _lensMode => LensSettings.OverrideModes.Perspective;

        protected override void UpdateZoom()
        {
            var currentFov = _cinemachineCamera.Lens.FieldOfView;
            _cinemachineCamera.Lens.FieldOfView = Mathf.SmoothDamp(currentFov, _targetZoomValue, 
                ref _zoomVelocity, _zoomSmoothTime, Mathf.Infinity, Time.deltaTime);
        }
    }
}
