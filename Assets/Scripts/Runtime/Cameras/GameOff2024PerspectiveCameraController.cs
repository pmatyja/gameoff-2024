using OCSFX.Utility;
using OCSFX.Utility.Attributes;
using Unity.Cinemachine;
using UnityEngine;

namespace Runtime.Cameras
{
    [RequireComponent(typeof(CinemachineRecomposer))]
    public class GameOff2024PerspectiveCameraController : GameOff2024CameraControllerBase
    {
        private CinemachineRecomposer _recomposer;
        
        [Header("Perspective Settings")]
        [SerializeField, MinMaxRange(0.5f,10f)] private Vector2 _zoomRange = new Vector2(1, 2);
        [SerializeField, Range(1,10)] private float _fov = 6;
        
        protected override Vector2 GetZoomRange() => _zoomRange;
        protected override LensSettings.OverrideModes _lensMode => LensSettings.OverrideModes.Perspective;


        protected override void InitializeZoom()
        {
            _targetZoomValue = _recomposer.ZoomScale;
        }

        protected override void UpdateZoom()
        {
            // var currentRadius = _orbitalFollow.Radius;
            // _orbitalFollow.Radius = Mathf.SmoothDamp(currentRadius, _targetZoomValue, 
            //     ref _zoomVelocity, _zoomSmoothTime, Mathf.Infinity, Time.deltaTime);

            var currentRecomposeZoom = _recomposer.ZoomScale;
            _recomposer.ZoomScale = Mathf.SmoothDamp(currentRecomposeZoom, _targetZoomValue, 
                ref _zoomVelocity, _zoomSmoothTime, Mathf.Infinity, Time.deltaTime);
        }

        protected override bool ResolveDependencies()
        {
            _recomposer = gameObject.GetOrAdd<CinemachineRecomposer>();
            
            return base.ResolveDependencies();
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            _orbitalFollow.OrbitStyle = CinemachineOrbitalFollow.OrbitStyles.Sphere;
            
            _cinemachineCamera.Lens.FieldOfView = _fov;
        }
    }
}
