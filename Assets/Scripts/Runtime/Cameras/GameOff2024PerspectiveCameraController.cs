using OCSFX.Attributes;
using OCSFX.Utility;
using OCSFX.Utility.Attributes;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Runtime.Cameras
{
    [RequireComponent(typeof(CinemachineRecomposer))]
    public class GameOff2024PerspectiveCameraController : GameOff2024CameraControllerBase
    {
        private CinemachineRecomposer _recomposer;
        
        [Header("Perspective Settings")]
        [SerializeField, MinMaxRange(0.5f,10f)] private Vector2 _zoomRange = new Vector2(1, 2);
        [SerializeField, Range(1,10)] private float _fov = 6;
        [SerializeField] private Volume _postProcessingVolume;
        [SerializeField, MinMaxRange(0,300)] private Vector2 _depthOfFieldFocalLengthRange = new Vector2(0, 300);
        private float _depthOfFieldFocalLengthVelocity;
        
        [Space]
        [SerializeField, ReadOnly, Range(0,300)] private float _depthOfFieldFocalLength = 100;

        protected override Vector2 GetZoomRange() => _zoomRange;
        protected override LensSettings.OverrideModes _lensMode => LensSettings.OverrideModes.Perspective;


        protected override void InitializeZoom()
        {
            _targetZoomValue = _recomposer.ZoomScale;

            if (!_postProcessingVolume)
            {
                _postProcessingVolume = GameOff2024Statics.GetGlobalPostProcessingVolume();
            }
            
            if (_postProcessingVolume && _postProcessingVolume.profile.TryGet(out DepthOfField depthOfField))
            {
                _depthOfFieldFocalLength = depthOfField.focalLength.value;
            }
        }

        protected override void UpdateZoom()
        {
            var currentRecomposeZoom = _recomposer.ZoomScale;
            _recomposer.ZoomScale = Mathf.SmoothDamp(currentRecomposeZoom, _targetZoomValue,
                ref _zoomVelocity, _zoomSmoothTime, Mathf.Infinity, Time.deltaTime);

            if (_postProcessingVolume && _postProcessingVolume.profile.TryGet(out DepthOfField depthOfField))
            {
                _depthOfFieldFocalLength = _targetZoomValue.Map(_zoomRange.x, _zoomRange.y, _depthOfFieldFocalLengthRange.y, _depthOfFieldFocalLengthRange.x);

                depthOfField.focalLength.value = Mathf.SmoothDamp(depthOfField.focalLength.value, _depthOfFieldFocalLength,
                    ref _depthOfFieldFocalLengthVelocity, _zoomSmoothTime, Mathf.Infinity, Time.deltaTime);
            }
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
