using System;
using FMODUnity;
using OCSFX.Utility.Debug;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Audio
{
    public class GameOff2024EventResponder : MonoBehaviour
    {
        [SerializeField] private bool _showDebug;
        
        [SerializeField] private UnityEvent _onCollectableCollected;
        [SerializeField] private UnityEvent _onGameSettingsChanged;
        [SerializeField] private UnityEvent<string, float> _onGameSettingsAudioValueChanged;
        [SerializeField] private AudioSettingIdConversion[] _audioSettingIdConversions;

        private void OnEnable()
        {
            EventBus.AddListener<CollectableEventParameters>(OnCollectableCollected);
            EventBus.AddListener<GameSettingsChangedEventParameters>(OnGameSettingsChanged);
        }

        private void OnDisable()
        {
            EventBus.RemoveListener<CollectableEventParameters>(OnCollectableCollected);
            EventBus.RemoveListener<GameSettingsChangedEventParameters>(OnGameSettingsChanged);
        }

        private void OnCollectableCollected(object sender, CollectableEventParameters info)
        {
            _onCollectableCollected?.Invoke();
            
            OCSFXLogger.Log($"[{nameof(GameOff2024EventResponder)}] {nameof(OnCollectableCollected)}", this, _showDebug);
        }
        
        private void OnGameSettingsChanged(object sender, GameSettingsChangedEventParameters info)
        {
            _onGameSettingsChanged?.Invoke();
            
            if (info.Subsystem == GameSettingsSubsystem.Audio)
            {
                var convertedId = GetVolumeSettingsId(info.Id);
                
                if (convertedId == null)
                {
                    OCSFXLogger.LogWarning($"[{nameof(GameOff2024EventResponder)}] {nameof(OnGameSettingsChanged)} : " +
                                    $"No {nameof(AudioSettingIdConversion)} found for {info.Id}", this, _showDebug);
                    return;
                }
                
                OCSFXLogger.Log($"[{nameof(GameOff2024EventResponder)}] {nameof(OnGameSettingsChanged)} :" +
                                $" Volume Setting changed : " +
                                $"(Id: {info.Id}, Value: {info.Value}, VolumeSetting: {convertedId})", this, _showDebug);
                
                var floatValue = (float)(object)info.Value;
                _onGameSettingsAudioValueChanged?.Invoke(convertedId, floatValue);
            }
        }
        
        private string GetVolumeSettingsId(string gameSettingsId)
        {
            foreach (var conversion in _audioSettingIdConversions)
            {
                var volumeSettingsId = conversion.GetVolumeSettingsId(gameSettingsId);
                if (volumeSettingsId != null)
                {
                    return volumeSettingsId;
                }
            }
            
            return null;
        }

        [Serializable]
        private class AudioSettingIdConversion
        {
            public string GameSettingsId;
            [ParamRef] public string VolumeSettingsId;
            
            public AudioSettingIdConversion(string gameSettingsId, string volumeSettingsId)
            {
                GameSettingsId = gameSettingsId;
                VolumeSettingsId = volumeSettingsId;
            }
            
            public string GetVolumeSettingsId(string gameSettingsId)
            {
                return GameSettingsId == gameSettingsId ? VolumeSettingsId : null;
            }
            
            public string GetGameSettingsId(string volumeSettingsId)
            {
                return VolumeSettingsId == volumeSettingsId ? GameSettingsId : null;
            }
        }
    }
}