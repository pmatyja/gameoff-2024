using System.Collections;
using UnityEngine;

namespace Runtime.Interactions
{
    public class InteractionPrompt : MonoBehaviour
    {
        [SerializeField] private bool _startHidden = true;
        [field: SerializeField, Readonly] public bool IsShowing { get; private set; }
        [SerializeField] private float _showDuration = 0.5f;
        
        private Coroutine _showCoroutine;
        
        private void Start()
        {
            if (_startHidden) Show(false, true);
        }
        
        public void Show(bool show)
        {
            if (IsShowing == show) return;
            AnimateShow(show);
            
            IsShowing = show;
        }
        
        public void Show(bool show, bool instant)
        {
            if (instant)
            {
                IsShowing = show;
                transform.localScale = show ? Vector3.one : Vector3.zero;
            }
            else
            {
                AnimateShow(show);
            }
        }

        private void AnimateShow(bool show)
        {
            if (_showCoroutine != null) StopCoroutine(_showCoroutine);
            _showCoroutine = StartCoroutine(Co_AnimateShow(show));
        }
        
        private IEnumerator Co_AnimateShow(bool show)
        {
            var startScale = show ? Vector3.zero : transform.localScale;
            var endScale = show ? Vector3.one : Vector3.zero;
            
            var timeElapsed = 0f;
            while (timeElapsed < _showDuration)
            {
                timeElapsed += Time.deltaTime;
                var progress = timeElapsed / _showDuration;
                transform.localScale = Vector3.Lerp(startScale, endScale, progress);
                yield return null;
            }
            
            IsShowing = show;
        }
    }
}