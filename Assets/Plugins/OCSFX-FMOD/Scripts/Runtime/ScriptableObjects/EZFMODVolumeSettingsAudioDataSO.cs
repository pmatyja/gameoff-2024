using System;
using System.Collections.Generic;
using OCSFX.EZFMOD.Types;
using OCSFX.EZFMOD.Debug;
using UnityEngine;

namespace OCSFX.EZFMOD.ScriptableObjects
{
    [CreateAssetMenu(menuName = _CREATE_ASSET_MENU_BASE + "Volume Settings", fileName = nameof(EZFMODVolumeSettingsAudioDataSO))]
    public class EZFMODVolumeSettingsAudioDataSO : EZFMODAudioDataSO
    {
        [field: SerializeField] public float DefaultValue { get; private set; } = 1.0f;
        [field: SerializeField] public float DefaultMasterValue { get; private set; } = 0.9f;

        [Space]
        [SerializeField] private bool _autoSyncPlayerPrefsData = true;
        
        [Space]
        [SerializeField] private EZFMODParameterValue _masterVolumeParameterValue;
        [SerializeField] private List<EZFMODParameterValue> _volumeParameterValues
            = new();

        [Space]
        [SerializeField] private AudioPlayerPrefs _audioPlayerPrefs;

        private void OnEnable()
        {
            Application.quitting += OnApplicationQuit;
        }

        private void OnApplicationQuit()
        {
            Application.quitting -= OnApplicationQuit;
            // Reset to default values when exiting play mode.
            
            if (Application.isEditor)
            {
                ResetDefaults();
            }
        }

        private void ResetDefaults()
        {
            foreach (var entry in _volumeParameterValues)
            {
                if (!entry) continue;
                    
                var settingName = entry.Parameter?.Name;
                var defaultValue = entry == _masterVolumeParameterValue
                    ? DefaultMasterValue
                    : DefaultValue;

                    
                OCSFXLogger.Log($"Set {settingName} to {defaultValue}", this, _showDebug);
                SetVolume(settingName, defaultValue);
                SetMute(settingName, false);
            }
        }

        public void LoadFromPlayerPrefs()
        {
            foreach (var entry in _volumeParameterValues)
            {
                if (!entry) continue;
                
                var settingName = entry.Parameter?.Name;
                
                OCSFXLogger.Log($"Set {settingName} to {_audioPlayerPrefs.GetValue(settingName)}", this, _showDebug);
                SetVolume(settingName, _audioPlayerPrefs.GetValue(settingName));
                SetMute(settingName, _audioPlayerPrefs.IsMuted(settingName));
            }
        }

        public void SetVolume(string parameterName, float value)
        {
            var result =
                _volumeParameterValues.Find(result => result?.Parameter.Name == parameterName);

            if (!result || string.IsNullOrWhiteSpace(result.Parameter.Name))
            {
                OCSFXLogger.LogWarning($"{parameterName} was not found in {this}.{nameof(_volumeParameterValues)}", this, _showDebug);
                return;
            }
            
            var paramName = result.Parameter.Name;
            
            if (string.IsNullOrWhiteSpace(paramName)) return;
            
            result.SetValue(value, true);
            
            _audioPlayerPrefs.SetValue(parameterName, result.Value);
            
            OCSFXLogger.Log($"Set {parameterName} to {result.Value}", this, _showDebug);

            result.SetValue(value, true);
        }

        public float GetVolume(string parameterName)
        {
            var result =
                _volumeParameterValues.Find(result => result?.Parameter.Name == parameterName);

            var paramName = result?.Parameter.Name;
            
            if (string.IsNullOrWhiteSpace(paramName))
            {
                OCSFXLogger.LogWarning($"{parameterName} was not found in {this}.{nameof(_volumeParameterValues)}", this, _showDebug);

                return DefaultValue;
            }

            return result.Value;
        }

        public void SetMute(string settingName, bool mute)
        {
            var result =
                _volumeParameterValues.Find(result => result?.Parameter.Name == settingName);

            if (!result || string.IsNullOrWhiteSpace(result.Parameter.Name))
            {
                OCSFXLogger.LogWarning($"{settingName} was not found in {this}.{nameof(_volumeParameterValues)}", this, _showDebug);
                return;
            }
            
            result.SetValue(mute ? 0 : _audioPlayerPrefs.GetValue(settingName), true);
            
            _audioPlayerPrefs.SetMute(settingName, mute);
        }

        public bool IsMuted(string settingName)
        {
            var result =
                _volumeParameterValues.Find(result => result?.Parameter.Name == settingName);

            if (!result || string.IsNullOrWhiteSpace(result.Parameter.Name))
            {
                OCSFXLogger.LogWarning($"{settingName} was not found in {this}.{nameof(_volumeParameterValues)}", this, _showDebug);
                return false;
            }

            return _audioPlayerPrefs.IsMuted(settingName);
        }

        private void OnValidate()
        {
            if (_volumeParameterValues == null || _volumeParameterValues.Count < 1) return;
            
            _audioPlayerPrefs ??= new AudioPlayerPrefs(DefaultValue, DefaultMasterValue);
            
            if (_autoSyncPlayerPrefsData) SyncPlayerPrefs();
            
            if (!EZFMODRuntimeStatics.StartupBanksLoaded) return;
            
            foreach (var entry in _volumeParameterValues)
            {
                SetVolume(entry.Parameter.Name, entry.Value);
            }
        }

        private void SyncPlayerPrefs()
        {
            _audioPlayerPrefs.Entries ??= new List<AudioVolumeSetting>();

            if (_audioPlayerPrefs.Entries.Count > _volumeParameterValues.Count)
            {
                var difference = _audioPlayerPrefs.Entries.Count - _volumeParameterValues.Count;
                _audioPlayerPrefs.Entries.RemoveRange(_volumeParameterValues.Count, difference);
            }

            for (var i = 0; i < _volumeParameterValues.Count; i++)
            {
                if (i < _audioPlayerPrefs.Entries.Count)
                {
                    _audioPlayerPrefs.Entries[i].Name = _volumeParameterValues[i]?.Parameter.Name;

                    var setValue = _audioPlayerPrefs.Entries[i] != null
                        ? _audioPlayerPrefs.Entries[i].Name == _masterVolumeParameterValue.Parameter.Name
                        ? DefaultMasterValue
                        : DefaultValue
                        : DefaultValue;
                    
                    _volumeParameterValues[i].SetValue(setValue, true);
                }
                else
                {
                    _audioPlayerPrefs.Entries.Add
                    (
                        new AudioVolumeSetting(
                            _volumeParameterValues[i]?.Parameter.Name, 
                            _volumeParameterValues[i] ? _volumeParameterValues[i].Value : DefaultValue)
                    );
                }
            }
        }

        [Serializable]
        private class AudioPlayerPrefs
        {
            private float _defaultValue = 1.0f;
            private float _defaultMasterValue = 0.9f;
            
            public AudioPlayerPrefs()
            {
                Entries =
                    new List<AudioVolumeSetting>
                    {
                        new AudioVolumeSetting("Volume_Master", _defaultMasterValue),
                        new AudioVolumeSetting("Volume_Music", _defaultValue),
                        new AudioVolumeSetting("Volume_SFX", _defaultValue),
                    };
            }
            
            public AudioPlayerPrefs(float defaultValue, float defaultMasterValue)
            {
                _defaultValue = defaultValue;
                _defaultMasterValue = defaultMasterValue;
                
                Entries =
                    new List<AudioVolumeSetting>
                    {
                        new AudioVolumeSetting("Volume_Master", _defaultMasterValue),
                        new AudioVolumeSetting("Volume_Music", _defaultValue),
                        new AudioVolumeSetting("Volume_SFX", _defaultValue),
                    };
            }
            
            [field: SerializeField]
            public List<AudioVolumeSetting> Entries { get; set; }

            public void SetValue(string name, float value)
            {
                var entry =
                    Entries.Find(entry => entry?.Name == name);
                if (string.IsNullOrWhiteSpace(entry?.Name)) return;

                entry.Value = value;
                PlayerPrefs.SetFloat(entry.Name, entry.Value);
            }

            public float GetValue(string name)
            {
                var entry =
                    Entries.Find(entry => entry?.Name == name);

                if (string.IsNullOrEmpty(entry?.Name)) return 1;
                
                return PlayerPrefs.GetFloat(entry.Name, _defaultValue);
            }
            
            public void SetMute(string name, bool mute)
            {
                var entry =
                    Entries.Find(entry => entry?.Name == name);
                if (string.IsNullOrWhiteSpace(entry?.Name)) return;

                entry.IsMuted = mute;
                PlayerPrefs.SetInt($"{entry.Name}_mute", entry.IsMuted ? 0 : 1);
            }
            
            public bool IsMuted(string name)
            {
                var entry =
                    Entries.Find(entry => entry?.Name == name);
                if (string.IsNullOrWhiteSpace(entry?.Name)) return false;

                return PlayerPrefs.GetInt($"{entry.Name}_mute", 1) == 0;
            }
        }

        [Serializable]
        private class AudioVolumeSetting
        {
            public string Name = "";
            public float Value = 1;
            public float DefaultValue = 1;
            public bool IsMuted = false;
            
            public AudioVolumeSetting(string name, float value, float defaultValue = 1, bool isMuted = false)
            {
                Name = name;
                Value = value;
                DefaultValue = !Mathf.Approximately(defaultValue, value) ? defaultValue : value;
                IsMuted = isMuted;
            }

            public AudioVolumeSetting()
            {
            }
        }
    }
}
