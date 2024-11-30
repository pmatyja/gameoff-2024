using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.SceneLoading
{
    public class ScreenFade : OCSFX.Generics.Singleton<ScreenFade>
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _image;
        [SerializeField, Min(0)] private float _defaultFadeDuration = 1f;
        [SerializeField, Min(0)] private float _defaultSustainDuration = 1f;
        [SerializeField] private Color _defaultFadeColor = Color.black;
        
        private Coroutine _fadeRoutine;
        
        protected override void Awake()
        {
            base.Awake();
            
            _canvasGroup.alpha = 0;
        }
        
        public void FadeIn(float duration = default, Color color = default, Action onComplete = null)
        {
            if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
            _fadeRoutine = StartCoroutine(Co_FadeIn(duration == default ? _defaultFadeDuration : duration, color, onComplete));
        }
        
        public void FadeOut(float duration = default, Color color = default, Action onComplete = null)
        {
            if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
            _fadeRoutine = StartCoroutine(Co_FadeOut(duration == default ? _defaultFadeDuration : duration, color, onComplete));
        }

        public void FadeInOut()
        {
            if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
            
            _fadeRoutine = StartCoroutine(Co_FadeInOut(_defaultFadeDuration, _defaultSustainDuration, _defaultFadeDuration, _defaultFadeColor, null, null));
        }
        
        public void FadeInOut(float fadeInDuration, float sustainDuration, float fadeOutDuration, Color color, 
            Action onFadeInComplete, Action onFadeOutComplete)
        {
            if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
            _fadeRoutine = StartCoroutine(Co_FadeInOut(fadeInDuration, sustainDuration, fadeOutDuration, color, onFadeInComplete, onFadeOutComplete));
        }

        private IEnumerator Co_FadeIn(float duration, Color color = default, Action onComplete = null)
        {
            if (color == default) color = _defaultFadeColor;
            _image.color = color;
            
            var timer = 0f;
            while (timer < duration)
            {
                _canvasGroup.alpha = Mathf.Lerp(0, 1, timer / duration);
                timer += Time.deltaTime;
                yield return null;
            }

            onComplete?.Invoke();
        }
        
        private IEnumerator Co_FadeOut(float duration, Color color = default, Action onComplete = null)
        {
            if (color == default) color = _defaultFadeColor;
            _image.color = color;
            
            var timer = 0f;
            while (timer < duration)
            {
                _canvasGroup.alpha = Mathf.Lerp(1, 0, timer / duration);
                timer += Time.deltaTime;
                yield return null;
            }

            onComplete?.Invoke();
        }
        
        private IEnumerator Co_FadeInOut(float fadeInDuration, float sustainDuration, float fadeOutDuration, Color color, 
            Action onFadeInComplete, Action onFadeOutComplete)
        {
            yield return Co_FadeIn(fadeInDuration, color, onFadeInComplete);
            yield return GameOff2024Statics.GetWaitForSeconds(sustainDuration);
            yield return Co_FadeOut(fadeOutDuration, color, onFadeOutComplete);
        }

        private void OnValidate()
        {
            if (!_canvasGroup) _canvasGroup = GetComponentInChildren<CanvasGroup>();
            if (!_image) _image = GetComponentInChildren<Image>();
            
            _image.color = _defaultFadeColor;
        }
    }
}