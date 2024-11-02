using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using OCSFX.FMOD.Components;
using OCSFX.FMOD.Types;
using OCSFX.Utility.Debug;

namespace OCSFX.FMOD.AudioData
{
    [CreateAssetMenu(menuName = _CREATE_ASSET_MENU_BASE + "Snapshots", fileName = nameof(SnapshotsAudioDataSO))]
    public class SnapshotsAudioDataSO : AudioDataSO
    {
        // Fields
        [Header("Mix State Snapshots")]
        [SerializeField] private List<FMODEvent> _states = new List<FMODEvent>()
        {
            new FMODEvent("None", new EventReference()),
            new FMODEvent("Dialog", new EventReference()),
            new FMODEvent("PauseMenu", new EventReference()),
            new FMODEvent("MainMenu", new EventReference()),
            new FMODEvent("Narration", new EventReference()),
            new FMODEvent("Silence", new EventReference())
        };

        [Header("Reverb Snapshots")]
        [SerializeField] private List<FMODEvent> _reverbs = new List<FMODEvent>()
        {
            new FMODEvent("None", new EventReference()),
            new FMODEvent("Interior_SmallRoom", new EventReference()),
            new FMODEvent("Interior_MediumRoom", new EventReference()),
            new FMODEvent("Interior_LargeRoom", new EventReference()),
            new FMODEvent("Interior_Hallway", new EventReference()),
            new FMODEvent("Exterior", new EventReference())
        };
        
        private readonly HashSet<ReverbZoneBase> _reverbZoneSet = new HashSet<ReverbZoneBase>();

        private ReverbZoneBase _currentReverbZone;

        // Properties
        public List<FMODEvent> States => _states;
        public List<FMODEvent> Reverbs => _reverbs;

        // Methods

        public void OnReverbZoneEntered(ReverbZoneBase enteredReverbZone)
        {
            _reverbZoneSet.Add(enteredReverbZone);
            if (!_currentReverbZone)
            {
                _currentReverbZone = enteredReverbZone;
                StartReverbZone(enteredReverbZone);
                return;
            }

            if (enteredReverbZone.Priority < _currentReverbZone.Priority) return;
            
            if (_currentReverbZone) StopReverbZone(_currentReverbZone);
            _currentReverbZone = enteredReverbZone;
            StartReverbZone(enteredReverbZone);
        }

        public void OnReverbZoneExited(ReverbZoneBase exitedReverbZone)
        {
            if (_currentReverbZone && _currentReverbZone == exitedReverbZone)
            {
                StopReverbZone(exitedReverbZone);
                _reverbZoneSet.Remove(exitedReverbZone);
            }

            ReverbZoneBase highestPriorityZone = null;
            
            foreach (var ambientZone in _reverbZoneSet)
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

            _currentReverbZone = highestPriorityZone;
            if (!_currentReverbZone) return;
            
            StartReverbZone(_currentReverbZone);
        }
        
        private void StartReverbZone(ReverbZoneBase reverbZone)
        {
            if (_reverbs.Count < 1) return;
            
            var reverbSnapshot = _reverbs.Find(reverb => reverb.Name == reverbZone.ReverbName);
            if (reverbSnapshot.EventRef.IsNull) return;

            OCSFXLogger.Log($"Start Reverb: Name: {reverbZone.ReverbName}", this, _showDebug);
            
            reverbSnapshot.EventRef.Play2D();
        }
        
        private void StopReverbZone(ReverbZoneBase reverbZone)
        {
            if (_reverbs.Count < 1) return;
            
            var reverbEvent = _reverbs.Find(reverb => reverb.Name == reverbZone.ReverbName);
            if (reverbEvent.EventRef.IsNull) return;
            
            reverbEvent.EventRef.StopGlobal();
        }
        
        public void StopAllReverbs()
        {
            if (_reverbs.Count > 0)
                foreach (var reverb in _reverbs) StopSnapshot(reverb.EventRef);
        }

        public void StopAllStates()
        {
            if (_states.Count > 0)
                foreach (var state in _states) StopSnapshot(state.EventRef);
        }

        public void ClearAllSnapshots()
        {
            StopAllStates();
            StopAllReverbs();
        }

        public void SetStateSnapshot(string stateName, bool state)
        {
            if (!_states.TryGetEventReference(stateName, out var eventRef))
            {
                OCSFXLogger.LogWarning($"{stateName} was not found in {_states}. Check {this}.", this, _showDebug);
                return;
            }

            if (state) StartSnapshot(eventRef);
            else StopSnapshot(eventRef);
        }

        public void SetReverbSnapshot(string reverbName, bool state)
        {
            if (!_reverbs.TryGetEventReference(reverbName, out var eventRef))
            {
                OCSFXLogger.LogWarning($"{reverbName} was not found in {_reverbs}. Check {this}.", this, _showDebug);
                return;
            }

            if (state) StartSnapshot(eventRef);
            else StopSnapshot(eventRef);
        }

        public void StartStateSnapshot(string eventKey)
        {
            var snapshot = _states.Find(match => match.Name == eventKey);
            if (snapshot.EventRef.IsNull) return;

            snapshot.EventRef.Play2D();
        }

        public void StopStateSnapshot(string eventKey)
        {
            var snapshot = _states.Find(match => match.Name == eventKey);
            if (snapshot.EventRef.IsNull) return;

            snapshot.EventRef.StopGlobal();
        }
        
        private void StartReverbSnapshot(string eventKey)
        {
            var snapshot = _reverbs.Find(match => match.Name == eventKey);
            if (snapshot.EventRef.IsNull) return;

            snapshot.EventRef.Play2D();
        }

        private void StopReverbSnapshot(string eventKey)
        {
            var snapshot = _reverbs.Find(match => match.Name == eventKey);
            if (snapshot.EventRef.IsNull) return;

            snapshot.EventRef.StopGlobal();
        }
        
        private static void StartSnapshot(EventReference snapshot)
        {
            if (!snapshot.IsNull) snapshot.Play2D();
        }

        private static void StopSnapshot(EventReference snapshot)
        {
            if (!snapshot.IsNull) snapshot.StopGlobal();
        }
        
    }
}
