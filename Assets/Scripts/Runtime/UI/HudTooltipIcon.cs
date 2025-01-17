using System;
using System.Collections;
using Runtime.Cameras;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class HudTooltipIcon : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private float _showIconFadeInDuration = 1f;
        [SerializeField] private float _showIconSustainDuration = 2f;
        [SerializeField] private float _showIconFadeOutDuration = 1f;
        
        [SerializeField] private Sprite _cameraRotateEnabledIcon;
        [SerializeField] private Sprite _cameraRotateDisabledIcon;
        
        [SerializeField] private Sprite _clickHintIcon;

        private CinemachineCamera _gameplayFreeCamera;
        private bool _isGameplayFreeCameraActive;
        
        private Coroutine _showIconCoroutine;
        
        private void Awake()
        {
            _iconImage.enabled = false;
        }

        private void OnEnable()
        {
            GameOff2024CameraEventsHandler.OnCameraActivatedEvent += OnCameraActivated;
            GameOff2024CameraEventsHandler.OnCameraDeactivatedEvent += OnCameraDeactivated;
            
            FtueUiController.OnMouseHoverClickable += ShowClickHint;
        }

        private void OnDisable()
        {
            GameOff2024CameraEventsHandler.OnCameraActivatedEvent -= OnCameraActivated;
            GameOff2024CameraEventsHandler.OnCameraDeactivatedEvent -= OnCameraDeactivated;
            
            FtueUiController.OnMouseHoverClickable -= ShowClickHint;
        }

        private void ShowClickHint()
        {
            Debug.Log($"Set icon to {nameof(_clickHintIcon)}", this);
            
            ShowIcon(_clickHintIcon);
        }
        
        private CinemachineCamera GetGameplayFreeCamera()
        {
            if (_gameplayFreeCamera) return _gameplayFreeCamera;

            var gameOff2024Camera = FindFirstObjectByType<GameOff2024CameraControllerBase>();
            if (!gameOff2024Camera) return null;
            
            _gameplayFreeCamera = gameOff2024Camera.GetCinemachineCamera();

            return _gameplayFreeCamera;
        }
        
        private void OnCameraActivated(ICinemachineMixer cinemachineMixer, ICinemachineCamera cinemachineCamera)
        {
            var cinemachineCam = (CinemachineCamera)cinemachineCamera;

            if (!cinemachineCam)
            {
                Debug.LogError($"Failed to cast {nameof(ICinemachineCamera)} to {nameof(CinemachineCamera)}", this);
                return;
            }

            var newCamIsFreeCam = IsGameplayFreeCamera(cinemachineCam);
            
            if (_isGameplayFreeCameraActive == newCamIsFreeCam) return;
            
            _isGameplayFreeCameraActive = newCamIsFreeCam;
            OnFreeCamStatusChanged();
        }

        private void OnFreeCamStatusChanged()
        {
            if (_isGameplayFreeCameraActive)
            {
                Debug.Log($"Set icon to {nameof(_cameraRotateEnabledIcon)}", this);
                
                ShowIcon(_cameraRotateEnabledIcon);
            }
            else
            {
                Debug.Log($"Set icon to {nameof(_cameraRotateDisabledIcon)}", this);
                
                ShowIcon(_cameraRotateDisabledIcon);
            }
        }

        private bool IsGameplayFreeCamera(CinemachineCamera other)
        {
            return other == GetGameplayFreeCamera();
        }

        private void OnCameraDeactivated(ICinemachineMixer cinemachineMixer, ICinemachineCamera cinemachineCamera)
        {

        }
        
        private void ShowIcon(Sprite icon)
        {
            if (_showIconCoroutine != null)
            {
                StopCoroutine(_showIconCoroutine);
            }
            
            _showIconCoroutine = StartCoroutine(Co_ShowIcon(icon));
        }
        
        private IEnumerator Co_ShowIcon(Sprite icon)
        {
            _iconImage.enabled = true;
            _iconImage.color = Color.clear;
            _iconImage.sprite = icon;
            
            var timer = 0f;
            
            var fadeDuration = _showIconFadeInDuration;
            
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                _iconImage.color = Color.Lerp(Color.clear, Color.white, timer / fadeDuration);
                yield return null;
            }
            
            timer = 0f;
            fadeDuration = _showIconSustainDuration;
            
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            
            timer = 0f;
            fadeDuration = _showIconFadeOutDuration;
            
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                _iconImage.color = Color.Lerp(Color.white, Color.clear, timer / fadeDuration);
                yield return null;
            }
            
            _iconImage.color = Color.clear;
            _iconImage.enabled = false;
        }
    }
}
