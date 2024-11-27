using System;
using System.Collections;
using OCSFX.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Utility
{
    public class DelayedUnityEvent : MonoBehaviour
    {
        [SerializeField] private TriggerOn _triggerOn = TriggerOn.Trigger;
        [SerializeField, Min(0)] private float _delay;
        [SerializeField] private bool _doOnce;
        [SerializeField] private UnityEvent _event;
        
        private bool _hasTriggered;

        private float _timer;

        private void Awake() => TriggerInternal(TriggerOn.Awake);
        private void Start() => TriggerInternal(TriggerOn.Start);
        private void OnEnable() => TriggerInternal(TriggerOn.Enable);
        private void OnDisable() => TriggerInternal(TriggerOn.Disable);
        private void OnDestroy() => TriggerInternal(TriggerOn.Destroy);

        public void Trigger()
        {
            if (!_triggerOn.HasFlag(TriggerOn.Trigger))
                Debug.LogWarning($"[{this}] Manual trigger is not enabled in TriggerOn flags.", this);
            else
                TriggerInternal(TriggerOn.Trigger);
        }

        private void TriggerInternal(TriggerOn triggerType)
        {
            if (_doOnce && _hasTriggered) return;

            if (!isActiveAndEnabled && triggerType != TriggerOn.Disable && triggerType != TriggerOn.Destroy) return;

            if (!_triggerOn.HasFlag(triggerType)) return;
            
            if (_doOnce) _hasTriggered = true;
            
            if (_delay <= 0)
            {
                _event?.Invoke();
            }
            else
            {
                if (triggerType == TriggerOn.Destroy)
                {
                    CoroutineRunner.CreateAndRun(Co_Delay());
                }
                else StartCoroutine(Co_Delay());   
            }
        }

        private IEnumerator Co_Delay()
        {
            var timer = 0f;

            while (timer < _delay)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            
            _event?.Invoke();
        }
        
        [Flags]
        private enum TriggerOn
        {
            Trigger = 2, 
            Awake = 4, 
            Start = 8, 
            Enable = 16, 
            Disable = 32, 
            Destroy = 64
        }
    }
}