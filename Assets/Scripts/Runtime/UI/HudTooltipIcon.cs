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
        [SerializeField] private Image _singleIconImage;
        [SerializeField] private Image[] _multipleIconImages;
        [SerializeField] private float _showIconFadeInDuration = 1f;
        [SerializeField] private float _showIconSustainDuration = 2f;
        [SerializeField] private float _showIconFadeOutDuration = 1f;
        
        [SerializeField] private Sprite _cameraRotateEnabledIcon;
        [SerializeField] private Sprite _cameraRotateDisabledIcon;
        
        [SerializeField] private Sprite _clickHintIcon;
        [SerializeField] private Sprite _scrollHintIcon;

        private CinemachineCamera _gameplayFreeCamera;
        private bool _isGameplayFreeCameraActive;
        
        private Coroutine _showIconCoroutine;
        
        private void Awake()
        {
            _singleIconImage.enabled = false;
            
            foreach (var image in _multipleIconImages)
            {
                image.enabled = false;
            }
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

        [ContextMenu(nameof(TestSingle))]
        private void TestSingle()
        {
            ShowIcon(_cameraRotateDisabledIcon);
        }
        
        [ContextMenu(nameof(TestMultiple))]
        private void TestMultiple()
        {
            ShowMultipleIcons(new []{ _cameraRotateEnabledIcon, _scrollHintIcon });
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
                
                ShowMultipleIcons(new []{ _cameraRotateEnabledIcon, _scrollHintIcon });
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
            
            SetVisible(false);
            
            _showIconCoroutine = StartCoroutine(Co_ShowIcon(icon));
        }
        
        private void ShowMultipleIcons(Sprite[] icons)
        {
            if (_showIconCoroutine != null)
            {
                StopCoroutine(_showIconCoroutine);
            }
            
            SetVisible(false);
            
            _showIconCoroutine = StartCoroutine(Co_ShowMultipleIcons(icons));
        }
        
        private void ShowIconsSequence(Sprite[] icons)
        {
            if (_showIconCoroutine != null)
            {
                StopCoroutine(_showIconCoroutine);
            }
            
            SetVisible(false);
            
            _showIconCoroutine = StartCoroutine(Co_ShowIconsSequence(icons));
        }
        
        private void SetVisible(bool visible)
        {
            _singleIconImage.enabled = visible;
            
            foreach (var image in _multipleIconImages)
            {
                image.enabled = visible;
            }
        }
        
        private IEnumerator Co_ShowIcon(Sprite icon)
        {
            _singleIconImage.enabled = true;
            _singleIconImage.color = Color.clear;
            _singleIconImage.sprite = icon;
            
            var timer = 0f;
            
            var fadeDuration = _showIconFadeInDuration;
            
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                _singleIconImage.color = Color.Lerp(Color.clear, Color.white, timer / fadeDuration);
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
                _singleIconImage.color = Color.Lerp(Color.white, Color.clear, timer / fadeDuration);
                yield return null;
            }
            
            _singleIconImage.color = Color.clear;
            _singleIconImage.enabled = false;
        }

        private IEnumerator Co_ShowIconsSequence(Sprite[] icons)
        {
            foreach (var icon in icons)
            {
                yield return Co_ShowIcon(icon);
            }
        }
        
        private IEnumerator Co_ShowMultipleIcons(Sprite[] icons)
        {
            if (icons.Length == 1)
            {
                yield return Co_ShowIcon(icons[0]);
                yield break;
            }
            
            for (var i = 0; i < _multipleIconImages.Length; i++)
            {
                var image = _multipleIconImages[i];
                image.enabled = true;
                image.color = Color.clear;
                image.sprite = icons[i];
            }
            
            var timer = 0f;
            
            var fadeDuration = _showIconFadeInDuration;
            
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                foreach (var image in _multipleIconImages)
                {
                    image.color = Color.Lerp(Color.clear, Color.white, timer / fadeDuration);
                }
                yield return null;
            }
            
            timer = 0f;
            fadeDuration = _showIconSustainDuration * icons.Length;
            
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
                foreach (var image in _multipleIconImages)
                {
                    image.color = Color.Lerp(Color.white, Color.clear, timer / fadeDuration);
                }
                yield return null;
            }
            
            foreach (var image in _multipleIconImages)
            {
                image.color = Color.clear;
                image.enabled = false;
            }
        }
    }
}
