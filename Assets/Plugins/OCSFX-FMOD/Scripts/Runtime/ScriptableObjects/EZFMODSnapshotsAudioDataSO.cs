using System.Collections.Generic;
using OCSFX.EZFMOD.Components;
using OCSFX.EZFMOD.Types;
using OCSFX.EZFMOD.Debug;
using OCSFX.EZFMOD.Utility.Generics;
using UnityEngine;

namespace OCSFX.EZFMOD.ScriptableObjects
{
    [CreateAssetMenu(menuName = _CREATE_ASSET_MENU_BASE + "Snapshots", fileName = nameof(EZFMODSnapshotsAudioDataSO))]
    public class EZFMODSnapshotsAudioDataSO : EZFMODAudioDataSO
    {
        // Fields
        [Header("Mix State Snapshots")]
        [SerializeField] private List<SerializedKeyValuePair<string, EZFMODSnapshot>> _stateSnapshots = new()
        {
            new SerializedKeyValuePair<string, EZFMODSnapshot>("None", null),
            new SerializedKeyValuePair<string, EZFMODSnapshot>("Silence", null),
            new SerializedKeyValuePair<string, EZFMODSnapshot>("MainMenu", null),
            new SerializedKeyValuePair<string, EZFMODSnapshot>("PauseMenu", null),
            new SerializedKeyValuePair<string, EZFMODSnapshot>("Cutscene", null),
            new SerializedKeyValuePair<string, EZFMODSnapshot>("Dialogue", null),
        };

        [Header("Reverb Snapshots")]
        [SerializeField] private List<SerializedKeyValuePair<string, EZFMODSnapshot>> _reverbSnapshots = new()
        {
            new SerializedKeyValuePair<string, EZFMODSnapshot>("None", null),
            new SerializedKeyValuePair<string, EZFMODSnapshot>("Exterior", null),
            new SerializedKeyValuePair<string, EZFMODSnapshot>("Interior_SmallRoom", null),
            new SerializedKeyValuePair<string, EZFMODSnapshot>("Interior_MediumRoom", null),
            new SerializedKeyValuePair<string, EZFMODSnapshot>("Interior_LargeRoom", null),
            new SerializedKeyValuePair<string, EZFMODSnapshot>("Interior_Hallway", null)
        };
        
        private readonly HashSet<EZFMODReverbZoneBase> _reverbZoneSet = new HashSet<EZFMODReverbZoneBase>();

        private EZFMODReverbZoneBase _currentReverbZone;

        // Properties
        public List<SerializedKeyValuePair<string, EZFMODSnapshot>> States => _stateSnapshots;
        public List<SerializedKeyValuePair<string, EZFMODSnapshot>> Reverbs => _reverbSnapshots;

        // Methods

        public void OnReverbZoneEntered(EZFMODReverbZoneBase enteredReverbZone)
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

        public void OnReverbZoneExited(EZFMODReverbZoneBase exitedReverbZone)
        {
            if (_currentReverbZone && _currentReverbZone == exitedReverbZone)
            {
                StopReverbZone(exitedReverbZone);
                _reverbZoneSet.Remove(exitedReverbZone);
            }

            EZFMODReverbZoneBase highestPriorityZone = null;
            
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
        
        private void StartReverbZone(EZFMODReverbZoneBase reverbZone)
        {
            if (!TryGetReverbSnapshot(reverbZone.ReverbName, out var reverbSnapshot)) return;

            OCSFXLogger.Log($"Start Reverb: Name: {reverbZone.ReverbName}", this, _showDebug);
            
            StartSnapshot(reverbSnapshot);
        }
        
        private void StopReverbZone(EZFMODReverbZoneBase reverbZone)
        {
            if (!TryGetReverbSnapshot(reverbZone.ReverbName, out var reverbSnapshot)) return;
            
            OCSFXLogger.Log($"Stop Reverb: Name: {reverbZone.ReverbName}", this, _showDebug);
            
            StopSnapshot(reverbSnapshot);
        }
        
        public void StopAllReverbs()
        {
            foreach (var reverb in _reverbSnapshots)
            {
                if (reverb == null) continue;
                StopSnapshot(reverb.Value);
            }
        }

        public void StopAllStates()
        {
            foreach (var state in _stateSnapshots)
            {
                if (state == null) continue;
                StopSnapshot(state.Value);
            }
        }

        public void ClearAllSnapshots()
        {
            StopAllStates();
            StopAllReverbs();
        }
        
        public void StartStateSnapshot(string stateName) => SetStateSnapshot(stateName, true);
        
        public void StopStateSnapshot(string stateName) => SetStateSnapshot(stateName, false);
        
        public void StartReverbSnapshot(string reverbName) => SetReverbSnapshot(reverbName, true);
        
        public void StopReverbSnapshot(string reverbName) => SetReverbSnapshot(reverbName, false);

        public void SetStateSnapshot(string stateName, bool state)
        {
            if (!TryGetStateSnapshot(stateName, out var snapshot))
            {
                OCSFXLogger.LogWarning($"{stateName} was not found in {_stateSnapshots}. Check {this}.", this, _showDebug);
                return;
            }

            if (state) StartSnapshot(snapshot);
            else StopSnapshot(snapshot);
        }

        public void SetReverbSnapshot(string reverbName, bool state)
        {
            if (!TryGetReverbSnapshot(reverbName, out var snapshot))
            {
                OCSFXLogger.LogWarning($"{reverbName} was not found in {_reverbSnapshots}. Check {this}.", this, _showDebug);
                return;
            }

            if (state) StartSnapshot(snapshot);
            else StopSnapshot(snapshot);
        }
        
        private static void StartSnapshot(EZFMODSnapshot snapshot)
        {
            if (!snapshot) return;
            snapshot.Play2D();
        }

        private static void StopSnapshot(EZFMODSnapshot snapshot)
        {
            if (!snapshot) return;
            snapshot.StopAll(true);
        }
        
        private bool TryGetStateSnapshot(string stateName, out EZFMODSnapshot snapshot)
        {
            snapshot = GetStateSnapshot(stateName);
            
            if (!snapshot)
            {
                OCSFXLogger.LogWarning($"{stateName} was not found in {_stateSnapshots}. Check {this}.", this, _showDebug);
            }
            
            return snapshot;
        }
        
        private EZFMODSnapshot GetStateSnapshot(string stateName)
        {
            return _stateSnapshots.Find(match => match.Key == stateName)?.Value;
        }
        
        private bool TryGetReverbSnapshot(string reverbName, out EZFMODSnapshot snapshot)
        {
            snapshot = GetReverbSnapshot(reverbName);
            
            if (!snapshot)
            {
                OCSFXLogger.LogWarning($"{reverbName} was not found in {_reverbSnapshots}. Check {this}.", this, _showDebug);
            }
            
            return snapshot;
        }
        
        private EZFMODSnapshot GetReverbSnapshot(string reverbName)
        {
            return _reverbSnapshots.Find(match => match.Key == reverbName)?.Value;
        }
    }
}
