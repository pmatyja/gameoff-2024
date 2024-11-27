using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using FMODUnity;
using OCSFX.Attributes;
using OCSFX.FMOD.Components;
using OCSFX.FMOD.Types;
using OCSFX.Utility.Debug;

namespace OCSFX.FMOD.AudioData
{
    [CreateAssetMenu(menuName = _CREATE_ASSET_MENU_BASE + "Ambience", fileName = nameof(AmbienceAudioDataSO))]
    public class AmbienceAudioDataSO : AudioDataSO
    {
        // Fields
        [SerializeField] private EventReference _ambDefault;
        [SerializeField] private bool _autoPlayAmbDefault;
        
        [Header("Ambience Events")]
        [SerializeField] private List<FMODEvent> _events = new List<FMODEvent>()
        {
            new FMODEvent("MainMenu", new EventReference()),
            new FMODEvent("Level01", new EventReference())
        };

        // private readonly List<EventReference> _eventHistory = new List<EventReference>();
        private readonly HashSet<AmbientZoneBase> _ambientZoneSet = new HashSet<AmbientZoneBase>();
        private readonly Stack<EventReference> _eventRefStack = new Stack<EventReference>();
        
        public List<FMODEvent> Events => _events;

        private EventInstance _currentPlayingAmb;
        [SerializeField, ReadOnly] private AmbientZoneBase _currentAmbientZone;

        // Methods

        public void StartAmbience2D(string eventName)
        {
            if (_events.Count < 1) return;
            
            var ambEvent = _events.Find(ambEvent => ambEvent.Name == eventName);
            if (ambEvent.EventRef.IsNull) return;

            OCSFXLogger.Log($"Start Amb: Name: {eventName}", this, _showDebug);
            
            if (_currentPlayingAmb.isValid())
                _currentPlayingAmb.Stop();
            
            _currentPlayingAmb = ambEvent.EventRef.Play2D();
        }

        public void StopAmbience2D(string eventName)
        {
            if (_events.Count < 1) return;
            
            var ambEvent = _events.Find(ambEvent => ambEvent.Name == eventName);
            if (ambEvent.EventRef.IsNull) return;

            if (_currentPlayingAmb.isValid() && _currentPlayingAmb.GetEventName() == ambEvent.EventRef.GetEventName())
                _currentPlayingAmb.Stop();
            else return;
            
            OCSFXLogger.Log($"Stop Amb: Name: {eventName}", this, _showDebug);
        }
        

        public void OnAmbientZoneEntered(AmbientZoneBase enteredAmbientZone)
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

        public void OnAmbientZoneExited(AmbientZoneBase exitedAmbientZone)
        {
            if (_currentAmbientZone && _currentAmbientZone == exitedAmbientZone)
            {
                StopAmbientZone(exitedAmbientZone);
                _ambientZoneSet.Remove(exitedAmbientZone);
            }

            AmbientZoneBase highestPriorityZone = null;
            
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

        private void StartAmbientZone(AmbientZoneBase ambientZone)
        {
            if (_events.Count < 1) return;
            
            var ambEvent = _events.Find(ambEvent => ambEvent.Name == ambientZone.AmbEventName);
            if (ambEvent.EventRef.IsNull) return;

            OCSFXLogger.Log($"Start Amb: Name: {ambientZone.AmbEventName}", this, _showDebug);
            
            ambEvent.EventRef.Play2D();
        }
        
        private void StopAmbientZone(AmbientZoneBase ambientZone)
        {
            if (_events.Count < 1) return;
            
            var ambEvent = _events.Find(ambEvent => ambEvent.Name == ambientZone.AmbEventName);
            if (ambEvent.EventRef.IsNull) return;
            
            ambEvent.EventRef.StopGlobal();
        }

        public void PlayDefaultAmbience()
        {
            if (_ambDefault.IsNull) return;
            
            OCSFXLogger.Log($"[{this}] Playing default amb event.", this);
            _ambDefault.Play2D();
        }

        public void StopAllAmbience()
        {
            foreach (var ambEvent in _events)
            {
                ambEvent.EventRef.StopGlobal();
            }
        }
        
        public EventReference GetAmbEventRef(string ambName)
        {
            return _events.GetEventReference(ambName);
        }
        
        public bool TryGetAmbEventRef(string ambName, out EventReference ambEventRef)
        {
            return _events.TryGetEventReference(ambName, out ambEventRef);
        }

        private void OnValidate()
        {
            if (_ambDefault.IsNull) return;
            if (_events.Count < 1) return;

            if (_events[0].EventRef.IsNull) _events[0] = 
                new FMODEvent("Default", _ambDefault);
        }
    }
}