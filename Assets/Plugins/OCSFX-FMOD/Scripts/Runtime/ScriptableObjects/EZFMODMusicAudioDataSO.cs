using System;
using System.Collections.Generic;
using System.Linq;
using FMOD;
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

        // private readonly HashSet<GUID> _eventGuids = new HashSet<GUID>();
        private EZFMODEvent _currentPlayingEvent;

        // Properties
        public List<SerializedKeyValuePair<string, EZFMODEvent>> Events => _musicEvents;
        public List<SerializedKeyValuePair<string, EZFMODParameter>> Parameters => _musicParameters;

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

        public void PlayMusicByKey(string musicEventName)
        {
            if (!TryGetMusicEvent(musicEventName, out var foundMusicEvent))
            {
                OCSFXLogger.LogWarning($"{this}: {musicEventName} was not found in MusicEvents.", this, _showDebug);
                return;
            }
            
            PlayMusic(foundMusicEvent);
        }
        
        public void StopCurrentMusic()
        {
            StopMusic();
        }
        
        private void PlayMusic(EZFMODEvent musicEvent)
        {
            if (musicEvent == _currentPlayingEvent)
            {
                OCSFXLogger.LogWarning($"[{this}] {musicEvent.Name} music is already playing.", this, _showDebug);
                return;
            }

            OCSFXLogger.Log("Play Music: " + musicEvent.Name, this, _showDebug);
            
            _currentPlayingEvent?.StopAll(true);
            
            musicEvent.Play2D();
            
            _currentPlayingEvent = musicEvent;
        }

        private void StopMusic()
        {
            if (!_currentPlayingEvent)
            {
                OCSFXLogger.LogWarning($"[{this}] No music is currently playing.", this, _showDebug);
                return;
            }
            
            OCSFXLogger.Log("Stop Music: " + _currentPlayingEvent.Name, this, _showDebug);
            
            _currentPlayingEvent.StopAll(true);
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