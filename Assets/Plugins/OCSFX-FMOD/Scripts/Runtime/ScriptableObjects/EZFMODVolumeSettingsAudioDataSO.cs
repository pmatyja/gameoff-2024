using System;
using System.Collections.Generic;
using OCSFX.EZFMOD.Types;
using OCSFX.EZFMOD.Debug;
using OCSFX.EZFMOD.Utility.Generics;
using UnityEngine;

namespace OCSFX.EZFMOD.ScriptableObjects
{
    [CreateAssetMenu(menuName = _CREATE_ASSET_MENU_BASE + "Volume Settings", fileName = nameof(EZFMODVolumeSettingsAudioDataSO))]
    public class EZFMODVolumeSettingsAudioDataSO : EZFMODAudioDataSO
    {
        private const float _DEFAULT_VALUE = 1.0f;
        private const float _DEFAULT_MASTER_VALUE = 0.9f;
        
        [SerializeField] private bool _autoFillPlayerPrefsData = true;

        [SerializeField] private List<SerializedKeyValuePair<EZFMODParameter, float>> _volumeParameterValues
            = new List<SerializedKeyValuePair<EZFMODParameter, float>>()
            {
                new SerializedKeyValuePair<EZFMODParameter, float>(null, _DEFAULT_MASTER_VALUE),
                new SerializedKeyValuePair<EZFMODParameter, float>(null, _DEFAULT_VALUE),
                new SerializedKeyValuePair<EZFMODParameter, float>(null, _DEFAULT_VALUE)
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
                foreach (var entry in _volumeParameterValues)
                {
                    _muteCaches.Set(entry.Key.Name, entry.Value);
                }
            }
            else
            {
                foreach (var entry in _volumeParameterValues)
                {
                    SetVolume(entry.Key.Name, _muteCaches.Get(entry.Key.Name));
                }
            }
        }

        public void LoadFromPlayerPrefs()
        {
            SetMuteCaches();
            
            foreach (var entry in _volumeParameterValues)
            {
                if (entry.Key == null) continue;
                
                var paramName = entry.Key?.Name;
                
                OCSFXLogger.Log($"Set {paramName} to {_audioPlayerPrefs.GetValue(paramName)}", this, _showDebug);
                SetVolume(paramName, _audioPlayerPrefs.GetValue(paramName));
            }
        }

        public void SetVolume(string parameterName, float value)
        {
            var result =
                _volumeParameterValues.Find(result => result.Key.Name == parameterName);

            if (result == null)
            {
                OCSFXLogger.LogWarning($"{parameterName} was not found in {this}.{nameof(_volumeParameterValues)}", this, _showDebug);
                return;
            }
            
            var paramName = result.Key?.Name;
            
            if (string.IsNullOrWhiteSpace(paramName)) return;
            
            result.Value = value;
            
            _audioPlayerPrefs.SetValue(parameterName, result.Value);
            
            OCSFXLogger.Log($"Set {parameterName} to {result.Value}", this, _showDebug);

            EZFMODRuntimeStatics.SetGlobalParameter(paramName, result.Value);
        }

        public float GetVolume(string parameterName)
        {
            var result =
                _volumeParameterValues.Find(result => result.Key.Name == parameterName);

            var paramName = result.Key?.Name;
            
            if (string.IsNullOrWhiteSpace(paramName))
            {
                OCSFXLogger.LogWarning($"{parameterName} was not found in {this}.{nameof(_volumeParameterValues)}", this, _showDebug);

                return _DEFAULT_VALUE;
            }

            result.Value = _audioPlayerPrefs.GetValue(parameterName);

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
            if (_volumeParameterValues == null || _volumeParameterValues.Count < 1) return;
            if (_autoFillPlayerPrefsData)
            {
                _audioPlayerPrefs ??= new AudioPlayerPrefs();
                _audioPlayerPrefs.Entries ??= new List<FMODGlobalParameter>();

                if (_audioPlayerPrefs.Entries.Count > _volumeParameterValues.Count)
                {
                    var difference = _audioPlayerPrefs.Entries.Count - _volumeParameterValues.Count;
                    _audioPlayerPrefs.Entries.RemoveRange(_volumeParameterValues.Count, difference);
                }

                for (var i = 0; i < _volumeParameterValues.Count; i++)
                {
                    if (i < _audioPlayerPrefs.Entries.Count)
                    {
                        _audioPlayerPrefs.Entries[i].Parameter = _volumeParameterValues[i].Key?.Name;
                    }
                    else{
                        _audioPlayerPrefs.Entries.Add(
                        new FMODGlobalParameter
                            (
                                _volumeParameterValues[i].Key.Name, _volumeParameterValues[i].Value
                            )
                        );
                        
                    }
                }
            }
            
            if (!EZFMODRuntimeStatics.MasterBanksLoaded) return;
            
            foreach (var entry in _volumeParameterValues)
            {
                SetVolume(entry.Key.Name, entry.Value);
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
