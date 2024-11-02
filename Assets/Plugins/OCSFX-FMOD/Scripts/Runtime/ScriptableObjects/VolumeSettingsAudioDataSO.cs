using System;
using System.Collections.Generic;
using UnityEngine;
using OCSFX.FMOD.Types;
using OCSFX.Utility.Debug;

namespace OCSFX.FMOD.AudioData
{
    [CreateAssetMenu(menuName = _CREATE_ASSET_MENU_BASE + "Volume Settings", fileName = nameof(VolumeSettingsAudioDataSO))]
    public class VolumeSettingsAudioDataSO : AudioDataSO
    {
        private const float _DEFAULT_VALUE = 1.0f;
        private const float _DEFAULT_MASTER_VALUE = 0.9f;
        
        [SerializeField] private bool _autoFillPlayerPrefsData = true;
        
        [SerializeField] private List<FMODGlobalParameter> _volumeSettingParameters =
            new List<FMODGlobalParameter>()
            {
                new FMODGlobalParameter("VolumeMaster", _DEFAULT_MASTER_VALUE),
                new FMODGlobalParameter("VolumeSfx", _DEFAULT_VALUE),
                new FMODGlobalParameter("VolumeMusic", _DEFAULT_VALUE)
            };

        [Space]
        [SerializeField] private AudioPlayerPrefs _audioPlayerPrefs = new AudioPlayerPrefs();

        private static MuteCaches _muteCaches;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            _muteCaches ??= new MuteCaches();
        }

        public void SetMuteCaches()
        {
            if (_muteCaches == null)
            {
                _muteCaches = new MuteCaches();
                foreach (var entry in _volumeSettingParameters)
                {
                    _muteCaches.Set(entry.Parameter, entry.Value);
                }
            }
            else
            {
                foreach (var entry in _volumeSettingParameters)
                {
                    SetVolume(entry.Parameter, _muteCaches.Get(entry.Parameter));
                }
            }
        }

        public void LoadFromPlayerPrefs()
        {
            SetMuteCaches();
            
            foreach (var entry in _volumeSettingParameters)
            {
                OCSFXLogger.Log($"Set {entry.Parameter} to {_audioPlayerPrefs.GetValue(entry.Parameter)}", this, _showDebug);
                SetVolume(entry.Parameter, _audioPlayerPrefs.GetValue(entry.Parameter));
            }
        }

        public void SetVolume(string key, float value)
        {
            var result =
                _volumeSettingParameters.Find(result => result.Parameter == key);

            if (result == null)
            {
                OCSFXLogger.LogWarning($"{key} was not found in {this}.{nameof(_volumeSettingParameters)}", this, _showDebug);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(result.Parameter)) return;
            
            result.Value = value;
            
            _audioPlayerPrefs.SetValue(key, result.Value);
            
            OCSFXLogger.Log($"Set {key} to {result.Value}", this, _showDebug);

            OCSFXAudioStatics.SetFMODParameterGlobal(result.Parameter, result.Value);
        }

        public float GetVolume(string key)
        {
            var result =
                _volumeSettingParameters.Find(result => result.Parameter == key);

            if (result == null || string.IsNullOrWhiteSpace(result.Parameter))
            {
                OCSFXLogger.LogWarning($"{key} was not found in {this}.{nameof(_volumeSettingParameters)}", this, _showDebug);

                return _DEFAULT_VALUE;
            }

            result.Value = _audioPlayerPrefs.GetValue(key);

            return result.Value;
        }

        public void SetMute(string key, bool mute)
        {
            _muteCaches ??= new MuteCaches();
            
            var newValue = mute ? 0f : _muteCaches.Get(key) > 0f ? _muteCaches.Get(key) : 1;

            SetVolume(key, newValue);

            if (!mute) _muteCaches.Set(key, GetVolume(key));
        }

        public bool IsMuted(string key)
        {
            return GetVolume(key) <= 0.001f;
        }

        private void OnValidate()
        {
            if (_volumeSettingParameters == null || _volumeSettingParameters.Count < 1) return;
            if (_autoFillPlayerPrefsData)
            {
                _audioPlayerPrefs ??= new AudioPlayerPrefs();
                _audioPlayerPrefs.Entries ??= new List<FMODGlobalParameter>();

                if (_audioPlayerPrefs.Entries.Count > _volumeSettingParameters.Count)
                {
                    var difference = _audioPlayerPrefs.Entries.Count - _volumeSettingParameters.Count;
                    _audioPlayerPrefs.Entries.RemoveRange(_volumeSettingParameters.Count, difference);
                }

                for (var i = 0; i < _volumeSettingParameters.Count; i++)
                {
                    if (i < _audioPlayerPrefs.Entries.Count)
                    {
                        _audioPlayerPrefs.Entries[i].Parameter = _volumeSettingParameters[i].Parameter;
                    }
                    else{
                        _audioPlayerPrefs.Entries.Add(
                        new FMODGlobalParameter
                            (
                            _volumeSettingParameters[i].Parameter, _volumeSettingParameters[i].Value
                            )
                        );
                        
                    }
                }
            }
            
            if (!OCSFXAudioStatics.StartupBanksAreLoaded) return;
            
            foreach (var entry in _volumeSettingParameters)
            {
                SetVolume(entry.Parameter, entry.Value);
            }
        }

        [Serializable]
        private class AudioPlayerPrefs
        {
            [field: SerializeField]
            public List<FMODGlobalParameter> Entries { get; set; } =
                new List<FMODGlobalParameter>()
                {
                    new FMODGlobalParameter("VolumeMaster", _DEFAULT_MASTER_VALUE),
                    new FMODGlobalParameter("VolumeSfx", _DEFAULT_VALUE),
                    new FMODGlobalParameter("VolumeMusic", _DEFAULT_VALUE)
                };

            public void SetValue(string key, float value)
            {
                var entry =
                    Entries.Find(entry => entry.Parameter == key);
                if (string.IsNullOrWhiteSpace(entry.Parameter)) return;

                entry.Value = value;
                PlayerPrefs.SetFloat(entry.Parameter, entry.Value);
            }

            public float GetValue(string key)
            {
                var entry =
                    Entries.Find(entry => entry.Parameter == key);

                return PlayerPrefs.GetFloat(entry.Parameter, _DEFAULT_VALUE);
            }
        }

        [Serializable]
        private class MuteCaches
        {
            [SerializeField] private bool _showDebug;
            
            public List<FMODGlobalParameter> Entries { get; private set; }
                = new List<FMODGlobalParameter>();

            public void Set(string key, float value)
            {
                var result =
                    Entries.Find(result => result.Parameter == key);

                if (string.IsNullOrWhiteSpace(result.Parameter))
                {
                    result = new FMODGlobalParameter(key, value);
                    Entries.Add(result);
                }

                result.Value = value;
            }

            public float Get(string key)
            {
                var result = Entries.Find(result => result.Parameter == key) 
                             ?? new FMODGlobalParameter(key, _DEFAULT_VALUE);

                if (string.IsNullOrWhiteSpace(result.Parameter))
                {
                    OCSFXLogger.LogWarning($"{key} was not found in {this}.{nameof(Entries)}. Using default value: {_DEFAULT_VALUE}", _showDebug);
                    result.Value = _DEFAULT_VALUE;
                }

                if (!Entries.Contains(result))
                {
                    Entries.Add(result);
                }

                return result.Value;
            }
        }
    }
}
