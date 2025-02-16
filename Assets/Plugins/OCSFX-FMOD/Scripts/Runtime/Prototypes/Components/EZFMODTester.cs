using System;
using System.Collections;
using System.Collections.Generic;
using OCSFX.EZFMOD.Attributes;
using OCSFX.EZFMOD.Debug;
using OCSFX.EZFMOD.Types;
using OCSFX.EZFMOD.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace OCSFX.EZFMOD.Prototypes.Components
{
    public class EZFMODTester : MonoBehaviour
    {
        [SerializeField] private EZFMODBank[] _banks;
        [SerializeField] private EZFMODEventBase _event;
        [SerializeField] private bool _playEventOnStart;
        private bool _started;
        [SerializeField] private bool _loopEvent;
        [SerializeField, Min(0.01f)] private float _loopEventInterval = 1.0f;
        private float _loopEventTimer;
        [SerializeField] private EZFMODParameter _parameter;
        [SerializeField] private bool _parameterIsTrigger;
        
        [Space]
        [SerializeField] private float fmodParameterValue;
        
        [field: Space]
        [field: SerializeField] public UnityEvent TestUnityEvent { get; private set; }

        [SerializeField, Button(nameof(TestUnityEventMethod))] private bool _testUnityEventButton;

        private bool AllBanksLoaded()
        {
            return _banks.Length == 0 || _loadedBanks.Count == _banks.Length;
        }

        private void Awake()
        {
            if (!AllBanksLoaded())
            {
                LoadBanks();
            }
        }

        private IEnumerator Start()
        {
            if (_playEventOnStart && AllBanksLoaded())
            {
                while(!EZFMODRuntimeStatics.StartupBanksLoaded)
                {
                    yield return null;
                }

                ApplyParameter();
                Play();
            }
        }

        public void TestUnityEventMethod()
        {
            TestUnityEvent?.Invoke();
        }

        private void Play()
        {
            if (!_event) return;
            
            _event.Play(gameObject);
            _started = true;
        }

        private void Update()
        {
            if (!_loopEvent || !_event) return;
            if (_playEventOnStart && !_started) return;
            
            if (_loopEventTimer < _loopEventInterval)
            {
                _loopEventTimer += Time.deltaTime;
                return;
            }
            
            _loopEventTimer = 0;
            Play();
        }

        private void LoadBanks()
        {
            if (AllBanksLoaded()) return;
            
            foreach (var bank in _banks)
            {
                if (!bank) continue;
                
                bank.LoadWithSampleData(OnBankLoaded);
            }
        }

        private readonly List<EZFMODBank> _loadedBanks = new();
        private void OnBankLoaded(EZFMODBank loadedBank)
        {
            if (_loadedBanks.Contains(loadedBank)) return;
            
            _loadedBanks.Add(loadedBank);

            if (!AllBanksLoaded()) return;
            
            OCSFXLogger.Log($"[{name}] All banks loaded!", this);
            OnAllBanksLoaded();
        }

        private void OnAllBanksLoaded()
        {
            if (!_playEventOnStart || !_event) return;
            
            OCSFXLogger.Log($"[{name}] Playing event ({_event.Name})!", this);
            
            ApplyParameter();
            Play();
        }

        private void ApplyParameter()
        {
            if (!_parameter) return;
            
            fmodParameterValue = Mathf.Clamp(fmodParameterValue, _parameter.Min, _parameter.Max);
            
            if (_parameter.IsGlobal)
            {
                _parameter.SetGlobalValue(fmodParameterValue);
            }
            else
            {
                _parameter.SetValue(fmodParameterValue, gameObject);
            }
                
            if (_parameterIsTrigger && !Mathf.Approximately(fmodParameterValue, _parameter.Default))
            {
                fmodParameterValue = _parameter.Default;
            }   
        }

        private void OnValidate()
        {
            if (!_parameter) return;
            fmodParameterValue = Mathf.Clamp(fmodParameterValue, _parameter.Min, _parameter.Max);
            ApplyParameter();
        }
    }
}