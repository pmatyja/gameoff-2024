using OCSFX.Utility;
using OCSFX.Utility.Attributes;
using Unity.Cinemachine;
using UnityEngine;

namespace Runtime.Cameras
{
    public class GameOff2024PerspectiveCameraController : GameOff2024CameraControllerBase
    {
        [Header("Perspective Settings")]
        [SerializeField, MinMaxRange(1,120)] private Vector2 _zoomRange = new Vector2(40, 100);
        [SerializeField, Range(1,10)] private float _fov = 6;
        
        protected override Vector2 GetZoomRange() => _zoomRange;
        protected override LensSettings.OverrideModes _lensMode => LensSettings.OverrideModes.Perspective;


        protected override void InitializeZoom()
        {
            _targetZoomValue = _orbitalFollow.Radius;
        }

        protected override void UpdateZoom()
        {
            var currentRadius = _orbitalFollow.Radius;
            _orbitalFollow.Radius = Mathf.SmoothDamp(currentRadius, _targetZoomValue, 
                ref _zoomVelocity, _zoomSmoothTime, Mathf.Infinity, Time.deltaTime);
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();

            _orbitalFollow.OrbitStyle = CinemachineOrbitalFollow.OrbitStyles.Sphere;
            
            _cinemachineCamera.Lens.FieldOfView = _fov;
        }
    }
}
