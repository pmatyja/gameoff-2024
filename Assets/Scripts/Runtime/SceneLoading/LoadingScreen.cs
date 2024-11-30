using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Runtime.SceneLoading
{
    public class LoadingScreen : OCSFX.Generics.Singleton<LoadingScreen>
    {
        [SerializeField, Range(0f, 1f)] private float _alpha = 1;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _header;
        [SerializeField] private Slider _progressBar;
        [SerializeField] private float _lerpSpeed = 2f;
        [SerializeField] private TMP_Text _percentage;
        [SerializeField] private string[] _ellipsisStages = { "",".", "..", "..." };
        private int _ellipsisStageIndex;

        [Header("Tooltips")]
        [SerializeField, Expandable] private TooltipCollectionSO _tooltipCollection;
        [SerializeField] private CanvasGroup _tooltipCanvasGroup;
        [SerializeField] private TMP_Text _tooltipHeader;
        [SerializeField] private TMP_Text _tooltipBody;

        private float _fadeDuration;
        private Coroutine _fadeRoutine;
        private Coroutine _progressBarRoutine;

        private Coroutine _animateHeaderRoutine;

        private bool _loadComplete;
        
        [Header("All-Platform Events")]
        [SerializeField] private UnityEvent _onLoadStarted;
        [SerializeField] private UnityEvent _onLoadCompleted;
        
        [Header("WebGL-Specific Events")]
        [SerializeField] private UnityEvent _onLoadStartedWebGL;
        [SerializeField] private UnityEvent _onLoadCompletedWebGL;


        private void OnEnable()
        {
            SceneLoadManager.LoadSceneQueued += OnLoadSceneQueued;
            SceneLoadManager.LoadSceneProgress += OnLoadSceneProgress;
            SceneLoadManager.LoadSceneComplete += OnLoadSceneComplete;
        }

        private void OnDisable()
        {
            SceneLoadManager.LoadSceneQueued -= OnLoadSceneQueued;
            SceneLoadManager.LoadSceneProgress -= OnLoadSceneProgress;
            SceneLoadManager.LoadSceneComplete -= OnLoadSceneComplete;
        }

        protected override void Awake()
        {
            base.Awake();
            Init();
        }

        private void Init()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;

            _progressBar.value = 0f;
            _percentage.text = "0%";
        }

        private void OnLoadSceneQueued(string sceneName, float transitionTime)
        {
            _loadComplete = false;
            _canvasGroup.blocksRaycasts = true;
            _progressBar.value = 0f;
            _percentage.text = "0%";
            _fadeDuration = transitionTime * 2;

    #if UNITY_WEBGL
            _onLoadStartedWebGL?.Invoke();
    #endif

            _onLoadStarted?.Invoke();

            FadeScreen(true, _fadeDuration);
            UpdateTooltip();
            AnimateHeader();
        }
        
        private void OnLoadSceneProgress(float progress)
        {
            UpdateProgressBar(progress);
        }

        private void OnLoadSceneComplete()
        {
            _canvasGroup.blocksRaycasts = false;
            
    # if UNITY_WEBGL
            _onLoadCompletedWebGL?.Invoke();        
    #endif
            
            _onLoadCompleted?.Invoke();
        }

        private void OnProgressBarFilled()
        {
            FadeScreen(false, _fadeDuration * 5);
            
            _loadComplete = true;
            if (_animateHeaderRoutine != null) StopCoroutine(_animateHeaderRoutine);
        }

        private void FadeScreen(bool visible, float duration)
        {
            if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
            
            _fadeRoutine = StartCoroutine(Co_FadeScreen(visible, duration));
        }

        private void UpdateProgressBar(float progress)
        {
            if (_progressBarRoutine != null) StopCoroutine(_progressBarRoutine);

            _progressBarRoutine = StartCoroutine(Co_UpdateProgressBar(progress));
        }

        private void UpdateTooltip()
        {
            if (!_tooltipCollection) return;

            var tooltip = _tooltipCollection.GetRandom();
            
            _tooltipHeader.text = tooltip.Header;
            _tooltipBody.text = tooltip.Text;
        }
        
        private void AnimateHeader() => StartCoroutine(Co_AnimateHeader());

        private IEnumerator Co_FadeScreen(bool visible, float duration)
        {
            var targetValue = visible ? 1f : 0f;
            var timer = 0f;

            while (Math.Abs(_canvasGroup.alpha - targetValue) > .01)
            {
                _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, targetValue, timer / duration);
                timer += Time.deltaTime;
                yield return null;
            }

            _canvasGroup.alpha = targetValue;
        }

        private IEnumerator Co_UpdateProgressBar(float progress)
        {
            while (Math.Abs(_progressBar.value - progress) > .01)
            {
                var lerpValue = Mathf.Lerp(_progressBar.value, progress, Time.deltaTime * _lerpSpeed);
                _progressBar.value = lerpValue;
                _percentage.text = $"{(int)(lerpValue * 100)}%";
                yield return null;
            }

            _progressBar.value = progress;
            _percentage.text = $"{(int)(progress * 100)}%";

            if (_progressBar.value > 0.89f)
            {
                OnProgressBarFilled();
            }
        }

        private IEnumerator Co_AnimateHeader()
        {
            var interval = 0.5f;
            var timer = 0f;

            while (timer < interval)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            _header.text = $"Loading{_ellipsisStages[_ellipsisStageIndex]}";

            if (_ellipsisStageIndex < _ellipsisStages.Length - 1) _ellipsisStageIndex++;
            else _ellipsisStageIndex = 0;
            
            if(!_loadComplete) AnimateHeader();
        }
        
        private T GetOrCreateChildComponent<T>(Transform parent, string gameObjectName) where T : Component
        {
            if (!parent) parent = transform;
            
            var child = parent.GetComponentInChildren<T>();
            if (child) return child;
            
            var go = new GameObject(typeof(T).Name);
            go.transform.SetParent(parent);
            go.name = gameObjectName;
            return go.AddComponent<T>();
        }

        private void OnValidate()
        {
            if (!_canvasGroup)
            {
                _canvasGroup = GetOrCreateChildComponent<CanvasGroup>(transform, "CanvasGroup");
            }
            
            _canvasGroup.alpha = _alpha;
            
            if (!_header)
            {
                _header = GetOrCreateChildComponent<TMP_Text>(_canvasGroup.transform, "Header");
                _header.text = "Loading";
            }
            
            if (!_progressBar)
            {
                _progressBar = GetOrCreateChildComponent<Slider>(_canvasGroup.transform, "ProgressBar");
                _progressBar.value = 0f;
            }
            
            if (!_percentage)
            {
                _percentage = GetOrCreateChildComponent<TMP_Text>(_canvasGroup.transform, "Percentage");
                if (_percentage == _header) // We want separate objects for header and percentage
                {
                    _percentage = new GameObject("Percentage").AddComponent<TMP_Text>();
                    _percentage.transform.SetParent(_canvasGroup.transform);
                }
                _percentage.text = "0%";
            }

            if (_tooltipCollection)
            {
                var randomTooltip = _tooltipCollection.GetRandom();
                if (!randomTooltip.IsValid()) return;
                
                if (_tooltipHeader)
                {
                    _tooltipHeader.text = randomTooltip.Header;
                }
                
                if (_tooltipBody)
                {
                    _tooltipBody.text = randomTooltip.Text;
                }
            }
        }
    }
}
