using System;
using System.Collections.Generic;
using UnityEngine;
using ArgumentNullException = System.ArgumentNullException;
using Object = UnityEngine.Object;

namespace OCSFX.EZFMOD.Utility
{
    /**<summary>
     * A simple timer utility class. For Runtime use only.
     * </summary>
     */
    public class Timer
    {
        private readonly float _duration;
        private float _timeElapsed;
        private int _loopCount;
        private int _loopsExecuted;
        
        private readonly Object _owner;
        
        private System.Action _onStart;
        private System.Action _onComplete;
        private System.Action _onTick;
        private System.Action _onStop;
        private System.Action _onPause;
        private System.Action _onResume;
        
        public float Duration => _duration;
        public float TimeElapsed => _timeElapsed;
        public bool IsRunning => TimerManager.IsTimerRegistered(this);

        private Timer(Object owner, float duration)
        {
            _duration = duration;
            _owner = owner;
        }
        
        public void Start()
        {
            _onStart?.Invoke();
            TimerManager.AddTimer(this);
        }
        
        public void Stop()
        {
            _timeElapsed = 0;
            _onStop?.Invoke();
            TimerManager.RemoveTimer(this);
        }
        
        public void Pause()
        {
            _onPause?.Invoke();
            TimerManager.RemoveTimer(this);
        }
        
        public void Resume()
        {
            TimerManager.AddTimer(this);
            _onResume?.Invoke();
        }

        private void Tick()
        {
            if (!_owner)
            {
                // Owner has been destroyed
                // UnityEngine.Debug.Log("Timer owner has been destroyed. Timer will be removed.");
                TimerManager.RemoveTimer(this);
                return;
            }
            
            _onTick?.Invoke();
            _timeElapsed += Time.deltaTime;
            
            if (_timeElapsed >= _duration)
            {
                OnDuration();
            }
        }
        
        private void OnDuration()
        {
            _timeElapsed = _duration;
            _onComplete?.Invoke();
                
            if (_loopCount > 0)
            {
                _loopsExecuted++;
                
                if (_loopsExecuted >= _loopCount)
                {
                    TimerManager.RemoveTimer(this);
                    return;
                }
            }
            else if (_loopCount == 0 || _loopCount == 1)
            {
                TimerManager.RemoveTimer(this);
                return;
            }
            
            _timeElapsed = 0;
        }
        
        /**<summary>
         * Creates a new Timer instance.
         * <param name="owner">The owner of the timer. This is used to ensure the timer is destroyed when the owner is destroyed to prevent memory leaks.
         * Recommended to use the current GameObject or MonoBehaviour for simplicity.
         * </param>
         * <param name="duration">The duration of the timer.</param>
         * </summary>
         */
        public static Builder Create(Object owner, float duration)
        {
            if (!owner)
            {
                throw new ArgumentNullException(nameof(owner), $"{typeof(Timer)} owner cannot be null.");
            }
            
            return new Builder(new Timer(owner, duration));
        }

        public class Builder
        {
            private readonly Timer _timer;
            private bool _isImmediateStart;

            public Builder(Timer timer)
            {
                _timer = timer;
            }
            
            /// <summary>
            /// Sets the callback to be invoked when the timer completes its duration.
            /// <param name="onComplete">The callback to be invoked each time the duration completes.</param>
            /// </summary>
            public Builder WithOnCompleteCallback(System.Action onComplete)
            {
                _timer._onComplete = onComplete;
                return this;
            }
            
            /// <summary>
            /// Sets the callback to be invoked when the timer starts.
            /// <param name="onStart">The callback to be invoked on start.</param>
            /// </summary>
            public Builder WithOnStartCallback(System.Action onStart)
            {
                _timer._onStart = onStart;
                return this;
            }

            /// <summary>
            /// Sets the callback to be invoked when the timer ticks (every frame).
            /// <param name="onTick">The callback to be invoked on update.</param>
            /// </summary>
            public Builder WithOnTickCallback(System.Action onTick)
            {
                _timer._onTick = onTick;
                return this;
            }

            /// <summary>
            /// Sets the callback to be invoked when the timer is stopped.
            /// <param name="onStop">The callback to be invoked on stop.</param>
            /// </summary>
            public Builder WithOnStopCallback(System.Action onStop)
            {
                _timer._onStop = onStop;
                return this;
            }

            /// <summary>
            /// Sets the callback to be invoked when the timer pauses.
            /// <param name="onPause">The callback to be invoked on pause.</param>
            /// </summary>
            public Builder WithOnPauseCallback(System.Action onPause)
            {
                _timer._onPause = onPause;
                return this;
            }

            /// <summary>
            /// Sets the callback to be invoked when the timer is resumed.
            /// <param name="onResume">The callback to be invoked on resume.</param>
            /// </summary>
            public Builder WithOnResumeCallback(System.Action onResume)
            {
                _timer._onResume = onResume;
                return this;
            }
            
            /**<summary>
             * Sets whether the timer should start immediately upon creation.
             * <param name="immediateStart">Whether the timer should start immediately upon creation. Default = TRUE.</param>
             * </summary>
             */
            public Builder WithImmediateStart(bool immediateStart = true)
            {
                _isImmediateStart = immediateStart;
                return this;
            }
            
            /**<summary>
             * Sets the number of times the timer should loop.
             * For simplicity, both 0 and 1 will not loop, 2 will loop once, etc. In other words (except for 0), the loop count is the number of times the timer will complete.
             * <param name="loopCount">The number of times the timer should loop. -1 = infinite, 0 or 1 completes once only. Default = -1 (infinite).</param>
             * </summary>
             */
            public Builder WithLoops(int loopCount = -1)
            {
                _timer._loopCount = loopCount;
                return this;
            }

            /**<summary>
             * Sets all TimerUnityEvents at once.
             * To set individual events, use the With(callback) methods.
             * <param name="timerUnityEvents">The TimerUnityEvents to set.</param>
             * </summary>
             */
            public Builder WithTimerUnityEvents(TimerUnityEvents timerUnityEvents)
            {
                _timer._onStart = timerUnityEvents.OnStart.Invoke;
                _timer._onComplete = timerUnityEvents.OnComplete.Invoke;
                _timer._onTick = timerUnityEvents.OnTick.Invoke;
                _timer._onStop = timerUnityEvents.OnStop.Invoke;
                _timer._onPause = timerUnityEvents.OnPause.Invoke;
                _timer._onResume = timerUnityEvents.OnResume.Invoke;
                return this;
            }

            public Timer Build()
            {
                if (_isImmediateStart)
                {
                    _timer.Start();
                }

                return _timer;
            }
        }

        private class TimerManager : MonoBehaviour
        {
            private static TimerManager _instance;
            private static TimerManager Instance
            {
                get
                {
                    if (!_instance)
                    {
                        _instance = new GameObject(nameof(TimerManager)).AddComponent<TimerManager>();
                        _instance.gameObject.hideFlags = HideFlags.HideAndDontSave;
                        DontDestroyOnLoad(_instance.gameObject);
                    }

                    return _instance;
                }
            }

            private void Awake()
            {
                if (_instance && _instance != this)
                {
                    Destroy(gameObject);
                    return;
                }
                
                _instance = this;
            }

            private void Update()
            {
                _timersThisFrame.UnionWith(_timers);

                foreach (var timer in _timersThisFrame)
                {
                    timer?.Tick();
                }

                _timersThisFrame.Clear();
            }

            private readonly HashSet<Timer> _timers = new HashSet<Timer>();
            private readonly HashSet<Timer> _timersThisFrame = new HashSet<Timer>();

            public static bool IsTimerRegistered(Timer timer)
            {
                // Runtime only
                if (!Application.isPlaying) return false;
                
                return Instance._timers.Contains(timer); 
            }

            public static void AddTimer(Timer timer)
            {
                // Runtime only
                if (!Application.isPlaying) return;
                
                Instance._timers.Add(timer);
            }

            public static void RemoveTimer(Timer timer)
            {
                // Runtime only
                if (!Application.isPlaying) return;
                
                Instance._timers.Remove(timer);
            }
        }

        [System.Serializable]
        public class TimerUnityEvents
        {
            public UnityEngine.Events.UnityEvent OnStart;
            public UnityEngine.Events.UnityEvent OnComplete;
            public UnityEngine.Events.UnityEvent OnTick;
            public UnityEngine.Events.UnityEvent OnStop;
            public UnityEngine.Events.UnityEvent OnPause;
            public UnityEngine.Events.UnityEvent OnResume;
        }
    }
}