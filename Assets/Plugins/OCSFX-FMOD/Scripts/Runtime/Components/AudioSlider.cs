using System;
using FMODUnity;
using OCSFX.Attributes;
using OCSFX.FMOD.AudioData;
using UnityEngine;
using UnityEngine.UI;

namespace OCSFX.FMOD.Components
{
    [RequireComponent(typeof(Slider))]
    [AddComponentMenu(OCSFXAudioStatics.CREATE_COMPONENT_MENU_BASE + nameof(AudioSlider))]
    public class AudioSlider : MonoBehaviour
    {
        [SerializeField] private VolumeSettingsAudioDataSO _volumeSettings;
        [SerializeField] private Slider _slider;
        [SerializeField, ParamRef] private string _fmodParameter;

        [Space]
        [Tooltip("PlayerPref name inherits the name of the FMOD parameter unless overridden below.")]
        [SerializeField] private Overrides _overrides;
        private string _playerPrefName;

        [field: SerializeField, ReadOnly] public bool IsMuted { get; private set; }
        
        private bool _valueLoaded;
        
        private void Awake()
        {
            LoadValue();
        }

        private void OnEnable()
        {
            if (!_slider) TryGetComponent(out _slider);
            _playerPrefName = _overrides.OverrideName ? _overrides.OverridePlayerPrefName : _fmodParameter;
            
            if (!OCSFXAudioStatics.StartupBanksAreLoaded)
                OCSFXAudioStatics.StartupBanksLoaded += OnMasterBanksLoaded;
            else LoadValue();
            
            _slider.onValueChanged.AddListener(SaveValue);
        }
        
        private void OnDisable()
        {
            _slider.onValueChanged.RemoveListener(SaveValue);
        }

        private void OnMasterBanksLoaded()
        {
            LoadValue();
            OCSFXAudioStatics.StartupBanksLoaded -= OnMasterBanksLoaded;
        }
    
        private void OnValidate()
        {
            if (!_slider) TryGetComponent(out _slider);
            _playerPrefName = _overrides.OverrideName ? _overrides.OverridePlayerPrefName : _fmodParameter;
            
            SaveValue(_slider.value);
        }
    
        private void SaveValue(float value)
        {
            if (!Application.isPlaying) return;
            if (!_valueLoaded) return;
            
            if (_overrides.OverrideName)
                PlayerPrefs.SetFloat(_playerPrefName, value);
            
            if (_volumeSettings)
            {
                _volumeSettings.SetVolume(_fmodParameter, value);
            }
            
            Debug.Log($"{this} loaded {_playerPrefName}: {_slider.value}", this);
        }

        private void LoadValue()
        {
            if (_overrides.OverrideName)
                _slider.value = PlayerPrefs.GetFloat(_playerPrefName, _slider.value);
            
            if (_volumeSettings)
            {
                var volume = _volumeSettings.GetVolume(_fmodParameter);

                _slider.value = volume;
                IsMuted = _volumeSettings.IsMuted(_fmodParameter);
            }
            
            _valueLoaded = true;
            SaveValue(_slider.value);
        }

        [System.Serializable]
        private struct Overrides
        {
            [SerializeField] private bool _overrideName;
            [SerializeField] private string _overridePlayerPrefName;
            
            public bool OverrideName
            {
                get => _overrideName;
                set => _overrideName = value;
            }
            
            public string OverridePlayerPrefName
            {
                get => _overridePlayerPrefName;
                set => _overridePlayerPrefName = value;
            }
        }
    }
}