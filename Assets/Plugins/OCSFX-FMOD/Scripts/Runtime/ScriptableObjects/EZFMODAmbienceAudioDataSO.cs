using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using OCSFX.EZFMOD.Components;
using OCSFX.EZFMOD;
using OCSFX.EZFMOD.Attributes;
using OCSFX.EZFMOD.Types;
using OCSFX.EZFMOD.Debug;
using OCSFX.EZFMOD.Utility.Generics;
using UnityEngine;

namespace OCSFX.EZFMOD.ScriptableObjects
{
    [CreateAssetMenu(menuName = _CREATE_ASSET_MENU_BASE + "Ambience", fileName = nameof(EZFMODAmbienceAudioDataSO))]
    public class EZFMODAmbienceAudioDataSO : EZFMODAudioDataSO
    {
        // Fields
        [SerializeField] private EZFMODEvent _ambDefaultEvent;
        [SerializeField] private bool _autoPlayAmbDefault;

        [SerializeField] private List<SerializedKeyValuePair<string, EZFMODEvent>> _ambEvents = new();

        // private readonly List<EventReference> _eventHistory = new List<EventReference>();
        private readonly HashSet<EZFMODAmbientZoneBase> _ambientZoneSet = new HashSet<EZFMODAmbientZoneBase>();
        private readonly Stack<EventReference> _eventRefStack = new Stack<EventReference>();
        
        public List<SerializedKeyValuePair<string, EZFMODEvent>> AmbEvents => _ambEvents;

        private EventInstance _currentPlayingAmb;
        private EZFMODAmbientZoneBase _currentAmbientZone;

        private void OnEnable()
        {
            if (_autoPlayAmbDefault) PlayDefaultAmbience();
            
            Application.quitting += ResetAll;
        }
        
        private void OnDisable()
        {
            ResetAll();
        }

        private void ResetAll()
        {
            Application.quitting -= ResetAll;
            
            StopAllAmbience();
            
            if (_currentAmbientZone)
            {
                StopAmbientZone(_currentAmbientZone);
                _currentAmbientZone = null;
            }
            
            _ambientZoneSet.Clear();
        }

        // Methods

        public void StartAmbience2D(string eventName)
        {
            if (!TryGetEvent(eventName, out var ambEvent)) return;

            OCSFXLogger.Log($"Start Amb: Name: {eventName}", this, _showDebug);
            
            if (_currentPlayingAmb.isValid())
                _currentPlayingAmb.Stop();
            
            ambEvent.Value.Play2D(out _currentPlayingAmb);
        }

        public void StopAmbience2D(string eventName)
        {
            if (!TryGetEvent(eventName, out var ambEvent)) return;

            if (_currentPlayingAmb.isValid() && _currentPlayingAmb.GetEventName() == ambEvent.Value.Name)
                _currentPlayingAmb.Stop();
            else return;
            
            OCSFXLogger.Log($"Stop Amb: Name: {eventName}", this, _showDebug);
        }
        

        public void OnAmbientZoneEntered(EZFMODAmbientZoneBase enteredAmbientZone)
        {
            _ambientZoneSet.Add(enteredAmbientZone);
            if (!_currentAmbientZone)
            {
                _currentAmbientZone = enteredAmbientZone;
                StartAmbientZone(enteredAmbientZone);
                return;
            }

            if (enteredAmbientZone.Priority < _currentAmbientZone.Priority) return;
            
            if (_currentAmbientZone) StopAmbientZone(_currentAmbientZone);
            _currentAmbientZone = enteredAmbientZone;
            StartAmbientZone(enteredAmbientZone);
        }

        public void OnAmbientZoneExited(EZFMODAmbientZoneBase exitedAmbientZone)
        {
            if (_currentAmbientZone && _currentAmbientZone == exitedAmbientZone)
            {
                StopAmbientZone(exitedAmbientZone);
                _ambientZoneSet.Remove(exitedAmbientZone);
            }

            EZFMODAmbientZoneBase highestPriorityZone = null;
            
            foreach (var ambientZone in _ambientZoneSet)
            {
                if (highestPriorityZone == null)
                {
                    highestPriorityZone = ambientZone;
                    continue;
                }
                
                if (ambientZone.Priority > highestPriorityZone.Priority)
                {
                    highestPriorityZone = ambientZone;
                }
            }

            _currentAmbientZone = highestPriorityZone;
            if (!_currentAmbientZone) return;
            
            StartAmbientZone(_currentAmbientZone);
        }

        private void StartAmbientZone(EZFMODAmbientZoneBase ambientZone)
        {
            if (!TryGetEvent(ambientZone.AmbEventName, out var ambEvent)) return;

            OCSFXLogger.Log($"Start Amb: Name: {ambientZone.AmbEventName}", this, _showDebug);
            
            ambEvent.Value.Play2D();
        }
        
        private void StopAmbientZone(EZFMODAmbientZoneBase ambientZone)
        {
            if (!TryGetEvent(ambientZone.AmbEventName, out var ambEvent)) return;

            ambEvent.Value.StopAll(true);
        }

        public void PlayDefaultAmbience()
        {
            if (!_ambDefaultEvent) return;
            
            OCSFXLogger.Log($"[{this}] Playing default amb event ({_ambDefaultEvent.Name}).", this);
            _ambDefaultEvent.Play2D();
        }

        public void StopAllAmbience()
        {
            if (!Application.isPlaying) return;
            
            foreach (var ambEvent in _ambEvents)
            {
                if (!ambEvent.Value) continue;
                
                ambEvent.Value.StopAll(true);
            }
        }
        
        private bool TryGetEvent(string eventName, out SerializedKeyValuePair<string, EZFMODEvent> ambEvent)
        {
            ambEvent = null;
            if (_ambEvents.Count < 1) return false;
            
            ambEvent = _ambEvents.Find(
                ambEvent => ambEvent.Key == eventName);
            if (ambEvent == null) return false;
            
            if (ambEvent.Value) return true;
            
            OCSFXLogger.LogError($"Entry ({eventName}) found, but it has no {nameof(EZFMODEvent)} assigned.", this);
            return false;
        }

        private void OnValidate()
        {
            if (!_ambDefaultEvent) return;
            if (_ambEvents.Count < 1) return;

            if (!_ambEvents[0].Value)
            {
                _ambEvents[0] = new SerializedKeyValuePair<string, EZFMODEvent>("Default", _ambDefaultEvent);
            }
        }
    }
}