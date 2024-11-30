using UnityEngine;
using UnityEngine.Events;

namespace Runtime.SceneLoading
{
    public class ScreenFader : MonoBehaviour
    {
        [SerializeField, Min(0)] private float _fadeInDuration = 1f;
        [SerializeField, Min(0)] private float _sustainDuration = 1f;
        [SerializeField, Min(0)] private float _fadeOutDuration = 1f;
        [SerializeField] private Color _fadeColor = Color.black;
        
        [field: SerializeField] public UnityEvent OnFadeInComplete { get; private set; }
        [field: SerializeField] public UnityEvent OnFadeOutComplete { get; private set; }
        
        public void FadeInOut()
        {
            var screenFade = GameOff2024Statics.GetScreenFade();
            if (!screenFade) return;
            
            screenFade.FadeInOut(_fadeInDuration, _sustainDuration, _fadeOutDuration, _fadeColor, 
                OnFadeInComplete.Invoke, OnFadeOutComplete.Invoke);
        }
        
        public void FadeIn()
        {
            var screenFade = GameOff2024Statics.GetScreenFade();
            if (!screenFade) return;
            
            screenFade.FadeIn(_fadeInDuration, _fadeColor, OnFadeInComplete.Invoke);
        }
        
        public void FadeOut()
        {
            var screenFade = GameOff2024Statics.GetScreenFade();
            if (!screenFade) return;
            
            screenFade.FadeOut(_fadeOutDuration, _fadeColor, OnFadeOutComplete.Invoke);
        }
    }
}