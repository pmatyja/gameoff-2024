using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class HudTooltipIcon : MonoBehaviour
    {
        [SerializeField] private Image _singleIconImage;
        [SerializeField] private Image[] _multipleIconImages;
        
        [Header("Settings")]
        [SerializeField] private float _showIconFadeInDuration = 1f;
        [SerializeField] private float _showIconSustainDuration = 2f;
        [SerializeField] private float _showIconFadeOutDuration = 1f;
        
        [Header("Icons")]
        [SerializeField] private Sprite _clickHintIcon;
        [SerializeField] private Sprite _rotateHintIcon;
        [SerializeField] private Sprite _scrollHintIcon;
        [SerializeField] private Sprite _cameraControlDisabledIcon;
        
        private Coroutine _showIconCoroutine;
        
        private Sprite[] _freeCameraHintIcons;
        
        private void Awake()
        {
            _singleIconImage.enabled = false;
            
            foreach (var image in _multipleIconImages)
            {
                image.enabled = false;
            }
            
            _freeCameraHintIcons = new[] { _rotateHintIcon, _scrollHintIcon };
        }

        private void OnEnable()
        {
            HintManager.OnHoverClickableEvent += OnHoverClickableHint;
            HintManager.OnRotateCameraInputEvent += OnRotateCameraInputHint;
            HintManager.OnZoomCameraInputEvent += OnZoomCameraInputHint;
            HintManager.OnGameOff2024CameraStatusChangedEvent += OnGameOff2024CameraStatusChangedEvent;
        }

        private void OnDisable()
        {
            HintManager.OnHoverClickableEvent -= OnHoverClickableHint;
            HintManager.OnRotateCameraInputEvent -= OnRotateCameraInputHint;
            HintManager.OnZoomCameraInputEvent -= OnZoomCameraInputHint;
            HintManager.OnGameOff2024CameraStatusChangedEvent -= OnGameOff2024CameraStatusChangedEvent;
        }

        private void OnGameOff2024CameraStatusChangedEvent(bool activeStatus)
        {
            if (!activeStatus) return;
            
            ShowMultipleIcons(_freeCameraHintIcons);
        }

        [ContextMenu(nameof(TestSingle))]
        private void TestSingle()
        {
            ShowIcon(_cameraControlDisabledIcon);
        }
        
        [ContextMenu(nameof(TestMultiple))]
        private void TestMultiple()
        {
            ShowMultipleIcons(new []{ _rotateHintIcon, _scrollHintIcon });
        }

        private void OnHoverClickableHint()
        {
            Debug.Log($"Set icon to {nameof(_clickHintIcon)}", this);
            
            ShowIcon(_clickHintIcon);
        }
        
        private void OnRotateCameraInputHint()
        {
            Debug.Log($"Set icon to {nameof(_cameraControlDisabledIcon)}", this);
            
            ShowIcon(_cameraControlDisabledIcon);
        }

        private void OnZoomCameraInputHint()
        {
            Debug.Log($"Set icon to {nameof(_cameraControlDisabledIcon)}", this);
            
            ShowIcon(_cameraControlDisabledIcon);
        }
        
        private void ShowIcon(Sprite icon)
        {
            if (IsSameDisplay(icon)) return;
            
            if (_showIconCoroutine != null)
            {
                StopCoroutine(_showIconCoroutine);
            }
            
            SetVisible(false);
            
            _showIconCoroutine = StartCoroutine(Co_ShowIcon(icon));
        }
        
        private void ShowMultipleIcons(Sprite[] icons)
        {
            if (IsSameDisplay(icons)) return;
            
            if (_showIconCoroutine != null)
            {
                StopCoroutine(_showIconCoroutine);
            }
            
            SetVisible(false);
            
            _showIconCoroutine = StartCoroutine(Co_ShowMultipleIcons(icons));
        }
        
        private void ShowIconsSequence(Sprite[] icons)
        {
            if (IsSameDisplay(icons)) return;
            
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
        
        private void ClearIcons()
        {
            _singleIconImage.sprite = null;
            
            foreach (var image in _multipleIconImages)
            {
                image.sprite = null;
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
            
            ClearIcons();
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
            
            ClearIcons();
        }
        
        private bool IsSameDisplay(Sprite sprite)
        {
            return _singleIconImage.sprite == sprite;
        }
        
        private bool IsSameDisplay(Sprite[] sprites)
        {
            if (sprites.Length != _multipleIconImages.Length) return false;
            
            for (var i = 0; i < sprites.Length; i++)
            {
                if (_multipleIconImages[i].sprite != sprites[i]) return false;
            }

            return true;
        }
    }
}
