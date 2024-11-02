using System;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using UnityEngine;
using FMODUnity;
using OCSFX.FMOD.Types;
using OCSFX.Utility.Debug;

namespace OCSFX.FMOD.AudioData
{    
    [CreateAssetMenu(menuName = _CREATE_ASSET_MENU_BASE + "Music", fileName = nameof(MusicAudioDataSO))]
    public class MusicAudioDataSO : AudioDataSO
    {
        [Header("Music Events")]
        [SerializeField] private List<FMODEvent> _events = new List<FMODEvent>()
        {
            new FMODEvent("MainMenu", new EventReference()),
            new FMODEvent("Gameplay", new EventReference()),
            new FMODEvent("PauseMenu", new EventReference()),
            new FMODEvent("GameOver", new EventReference()),
        };

        [Header("Music Global Parameters")]
        [SerializeField] private List<FMODGlobalParameter> _globalParameters = new List<FMODGlobalParameter>()
        {
            new FMODGlobalParameter("MusicParamVertical", 0),
            new FMODGlobalParameter("MusicParamHorizontal", 0) 
        };

        private readonly Dictionary<Guid, EventInstance> _instances = new Dictionary<Guid, EventInstance>();
        private EventInstance _currentInstance;
        private EventReference _currentEventRef;

        // Properties
        public List<FMODEvent> Events => _events;
        public List<FMODGlobalParameter> GlobalParameters => _globalParameters;
        public EventInstance CurrentInstance => _currentInstance;

        // Methods

        private bool TryGetMusicEvent(string musicEventName, out EventReference musicEventRef)
        {
            musicEventRef = GetMusicEvent(musicEventName);
            return !musicEventRef.IsNull;
        }
        
        private EventReference GetMusicEvent(string musicEventName)
        {
            var found = _events.Find(fmodEventStruct =>
                fmodEventStruct.Name == musicEventName).EventRef;

            return found;
        }

        public void CurrentMusicEventPlay()
        {
            if (_currentEventRef.IsNull)
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
            
            PlayMusic(_currentEventRef);
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
        
        private void PlayMusic(EventReference eventRef)
        {
            var eventRefID = eventRef.Guid;
            if (_instances.ContainsKey(eventRefID))
            {
                if (_instances[eventRefID].isValid())
                {
                    OCSFXLogger.LogWarning($"[{this}] {eventRef.GetEventName()} music is already playing.", this, _showDebug);
                }
                else
                {
                    _instances.Remove(eventRefID);
                }
            }

            if (_currentInstance.isValid())
            {
                _currentInstance.getDescription(out var eventDesc);
                eventDesc.getID(out var guid);
                
                if (_instances.ContainsKey(guid))
                {
                    _instances[guid].Stop();
                    _instances.Remove(guid);
                }
            }

            _currentEventRef = eventRef;
            _currentInstance = eventRef.Play2D();
            _instances.Add(eventRefID, _currentInstance);

            if (!_currentInstance.isValid())
            {
                OCSFXLogger.LogWarning($"[{this}] failed to play music {eventRef.GetEventName()}", this, _showDebug);
                return;
            }
            
            OCSFXLogger.Log("Play Music: " + eventRef.GetEventName(), this, _showDebug);
        }

        private void StopMusic(EventReference eventRef)
        {
            var eventRefID = eventRef.Guid;
            if (!_instances.TryGetValue(eventRefID, out var instance)) return;

            instance.Stop();
            _instances.Remove(eventRefID);
            
            OCSFXLogger.Log("Stop Music: " + eventRef.GetEventName(), this, _showDebug);
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
            if (!_globalParameters.TryGetParameter(parameterName, out var parameter))
            {
                OCSFXLogger.LogError($"{this}: {parameterName} was not found in GlobalParameters.", this, _showDebug);
                return;
            }
            
            OCSFXAudioStatics.SetFMODParameterGlobal(parameter, value);
        }

        public void ResetGlobalParameters()
        {
            foreach (var entry in _globalParameters)
            {
                OCSFXAudioStatics.SetFMODParameterGlobal(entry.Parameter, default);
            }
        }
        
        private void ApplyGlobalParameters()
        {
            foreach (var param in _globalParameters
                         .Where(param => !string.IsNullOrWhiteSpace(param.Parameter)))
            {
                OCSFXAudioStatics.SetFMODParameterGlobal(param.Parameter, param.Value);
            }
        }

        protected void OnValidate()
        {
            ApplyGlobalParameters();
        }
    }
}