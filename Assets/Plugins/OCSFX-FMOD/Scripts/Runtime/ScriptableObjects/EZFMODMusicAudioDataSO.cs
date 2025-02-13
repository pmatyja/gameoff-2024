using System;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using FMODUnity;
using OCSFX.EZFMOD.Types;
using OCSFX.EZFMOD.Debug;
using OCSFX.EZFMOD.Utility.Generics;
using UnityEngine;

namespace OCSFX.EZFMOD.ScriptableObjects
{    
    [CreateAssetMenu(menuName = _CREATE_ASSET_MENU_BASE + "Music", fileName = nameof(EZFMODMusicAudioDataSO))]
    public class EZFMODMusicAudioDataSO : EZFMODAudioDataSO
    {
        [SerializeField] private List<SerializedKeyValuePair<string, EZFMODEvent>> _musicEvents = new()
        {
            new SerializedKeyValuePair<string, EZFMODEvent>("MainMenu", null),
            new SerializedKeyValuePair<string, EZFMODEvent>("Gameplay", null),
            new SerializedKeyValuePair<string, EZFMODEvent>("PauseMenu", null),
            new SerializedKeyValuePair<string, EZFMODEvent>("GameOver", null),
        };
        
        [SerializeField] private List<SerializedKeyValuePair<string, EZFMODParameter>> _musicParameters = new()
        {
            new SerializedKeyValuePair<string, EZFMODParameter>("MusicParamVertical", null),
            new SerializedKeyValuePair<string, EZFMODParameter>("MusicParamHorizontal", null)
        };

        private readonly Dictionary<Guid, EventInstance> _instances = new Dictionary<Guid, EventInstance>();
        private EventInstance _currentInstance;
        private EZFMODEvent _currentEvent;

        // Properties
        public List<SerializedKeyValuePair<string, EZFMODEvent>> Events => _musicEvents;
        public List<SerializedKeyValuePair<string, EZFMODParameter>> Parameters => _musicParameters;
        public EventInstance CurrentInstance => _currentInstance;

        // Methods

        private bool TryGetMusicEvent(string musicEventName, out EZFMODEvent musicEvent)
        {
            musicEvent = GetMusicEventByName(musicEventName);
            return musicEvent;
        }
        
        private EZFMODEvent GetMusicEventByName(string musicEventName)
        {
            return _musicEvents.FirstOrDefault(musicEvent 
                => musicEvent.Key == musicEventName)?.Value;
        }

        public void CurrentMusicEventPlay()
        {
            if (!_currentEvent)
            {
                OCSFXLogger.LogWarning($"[{this}] No Current Music Event has been set.", this, _showDebug);
                return;
            }

            var canPlay = false;

            if (!_currentInstance.isValid()) canPlay = true;
            else
            {
                _currentInstance.getPlaybackState(out var playbackState);

                switch (playbackState)
                {
                    default:
                    case PLAYBACK_STATE.STOPPED: 
                    case PLAYBACK_STATE.STOPPING:
                        canPlay = true;
                        break;
                    case PLAYBACK_STATE.PLAYING:
                    case PLAYBACK_STATE.SUSTAINING:
                    case PLAYBACK_STATE.STARTING:
                        break;
                }   
            }

            if (!canPlay) return;
            
            PlayMusic(_currentEvent);
        }

        public void MusicEventPlay(string musicEventName)
        {
            if (!TryGetMusicEvent(musicEventName, out var foundMusicEvent))
            {
                OCSFXLogger.LogWarning($"{this}: {musicEventName} was not found in MusicEvents.", this, _showDebug);
                return;
            }
            
            PlayMusic(foundMusicEvent);
        }
        
        public void MusicEventStop(string musicEventName)
        {
            if (!TryGetMusicEvent(musicEventName, out var foundMusicEvent))
            {
                OCSFXLogger.LogWarning($"{this}: {musicEventName} was not found in MusicEvents.", this, _showDebug);
                return;
            }
            
            StopMusic(foundMusicEvent);
        }
        
        private void PlayMusic(EZFMODEvent musicEvent)
        {
            var eventID = musicEvent.GUID;

            if (_currentInstance.isValid())
            {
                var currentInstanceID = _currentInstance.GetEventGUID();

                if (eventID == currentInstanceID)
                {
                    OCSFXLogger.LogWarning($"[{this}] {musicEvent.Name} music is already playing.", this, _showDebug);
                    return;
                }

                if (_instances.TryGetValue(currentInstanceID, out var currentInstance))
                {
                    currentInstance.Stop();
                    _instances.Remove(currentInstanceID);
                }
            }

            _currentEvent = musicEvent;
            musicEvent.Play2D(out _currentInstance);
            
            // Add or set
            if (!_instances.TryAdd(eventID, _currentInstance))
            {
                _instances[eventID] = _currentInstance;
            }

            if (!_currentInstance.isValid())
            {
                OCSFXLogger.LogWarning($"[{this}] failed to play music {musicEvent.Name}", this, _showDebug);
                return;
            }

            OCSFXLogger.Log("Play Music: " + musicEvent.Name, this, _showDebug);
        }

        private void StopMusic(EZFMODEvent musicEvent)
        {
            var eventID = musicEvent.GUID;
            if (!_instances.TryGetValue(eventID, out var instance)) return;

            instance.Stop();
            _instances.Remove(eventID);
            
            OCSFXLogger.Log("Stop Music: " + musicEvent.Name, this, _showDebug);
        }
        
        public void StopAllMusic()
        {
            foreach (var entry in _instances) entry.Value.Stop();
            _instances.Clear();
        }

        public void SetLocalParameter(EventReference musicEventRef, string parameterName, float parameterValue)
        {
            if (!_instances.TryGetValue(musicEventRef.Guid, out var instance))
            {
                OCSFXLogger.LogError($"{musicEventRef.GetEventName()} not found in {this} instances.", this, _showDebug);
                return;
            }
            
            OCSFXLogger.Log($"Set {musicEventRef.GetEventName()} parameter {parameterName} value to {parameterValue}.", this, _showDebug);
            instance.setParameterByName(parameterName, parameterValue);
        }

        public void SetGlobalParameter(string parameterName, float value)
        {
            if (!TryGetMusicParameter(parameterName, out var parameter))
            {
                OCSFXLogger.LogWarning($"{this}: {parameterName} was not found in MusicParameters.", this, _showDebug);
                return;
            }
            
            parameter.SetGlobalValue(value);
        }
        
        private bool TryGetMusicParameter(string parameterName, out EZFMODParameter musicParameter)
        {
            musicParameter = GetMusicParameterByName(parameterName);
            return musicParameter;
        }
        
        private EZFMODParameter GetMusicParameterByName(string parameterName)
        {
            return _musicParameters.FirstOrDefault(param 
                => param.Key == parameterName)?.Value;
        }

        public void ResetParameters()
        {
            foreach (var param in _musicParameters)
            {
                param?.Value?.SetGlobalDefaultValue();
            }
        }
    }
}