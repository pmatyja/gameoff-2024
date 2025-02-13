using UnityEngine;

namespace OCSFX.EZFMOD.Utility
{
    public class TimerComponent : MonoBehaviour
    {
        [SerializeField] private float _duration = 1f;
        [SerializeField] private int _loops = -1;
        [SerializeField] private bool _startOnAwake = false;

        public Timer.TimerUnityEvents UnityEvents;
        
        private Timer _timer;
        
        protected virtual void Awake()
        {
            _timer = Timer.Create(this, _duration)
                .WithLoops(_loops)
                .WithImmediateStart(_startOnAwake)
                .WithTimerUnityEvents(UnityEvents)
                .Build();
        }
        
        [ContextMenu(nameof(StartTimer))]
        public void StartTimer() => _timer?.Start();
        
        [ContextMenu(nameof(StopTimer))]
        public void StopTimer() => _timer?.Stop();
        
        [ContextMenu(nameof(PauseTimer))]
        public void PauseTimer() => _timer?.Pause();
        
        [ContextMenu(nameof(ResumeTimer))]
        public void ResumeTimer() => _timer?.Resume();
        
        [ContextMenu(nameof(RestartTimer))]
        public void RestartTimer() { _timer?.Stop() ; _timer?.Start(); }
    }
}