using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class HudTooltipIcon : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        
        [SerializeField] private Sprite _cameraRotateEnabledIcon;
        [SerializeField] private Sprite _cameraRotateDisabledIcon;
        [SerializeField] private Sprite _cameraZoomEnabledIcon;
        [SerializeField] private Sprite _cameraZoomDisabledIcon;
        
        [SerializeField] private Sprite _clickHintIcon;

        private void OnEnable()
        {
            GameOff2024CameraEventsHandler.OnCameraActivatedEvent += OnCameraActivated;
            GameOff2024CameraEventsHandler.OnCameraDeactivatedEvent += OnCameraDeactivated;
        }

        private void OnDisable()
        {
            GameOff2024CameraEventsHandler.OnCameraActivatedEvent -= OnCameraActivated;
            GameOff2024CameraEventsHandler.OnCameraDeactivatedEvent -= OnCameraDeactivated;
        }
        
        private void OnCameraActivated(ICinemachineMixer cinemachineMixer, ICinemachineCamera cinemachineCamera)
        {

        }

        private void OnCameraDeactivated(ICinemachineMixer cinemachineMixer, ICinemachineCamera cinemachineCamera)
        {

        }
        
        private void SetIcon(Sprite icon)
        {
            _iconImage.sprite = icon;
        }
    }
}
