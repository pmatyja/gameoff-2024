using System;
using OCSFX.FMOD.AudioData;
using OCSFX.Utility;
using OCSFX.Attributes;
using UnityEngine;

namespace OCSFX.FMOD.Components
{
    public abstract class ReverbZoneBase: MonoBehaviour
    {
        [SerializeField] protected SnapshotsAudioDataSO _snapshotsAudioData;
        [Tooltip("If set, only objects with these layers will trigger overlaps.")]
        [SerializeField] protected LayerMask _layerMask;
        [Tooltip("Corresponds to a reverb on the SnapshotsAudioData.")]
        [SerializeField] protected string _reverbName = "Interior_SmallRoom";
        [SerializeField, Range(0f, 100f)] private float _priority = 1f;
        
        [SerializeField, ReadOnly] private int _overlapCount;
        
        public string ReverbName => _reverbName;
        public float Priority => _priority;
        
        protected void HandleTriggerEnter(Component other)
        {
            if (!_layerMask.Contains(other)) return;
            
            if (_overlapCount < 1)
            {
                if (_snapshotsAudioData)
                {
                    _snapshotsAudioData.OnReverbZoneEntered(this);
                }
            }
            
            _overlapCount++;
        }

        protected void HandleTriggerExit(Component other)
        {
            if (!_layerMask.Contains(other)) return;

            _overlapCount--;
            
            //if (_overlapCount < 1) _snapshotsAudioData.SetReverbSnapshot(_reverbName, false);
            
            if (_overlapCount >= 1) return;
            
            if (!_snapshotsAudioData) return;
            _snapshotsAudioData.OnReverbZoneExited(this);
        }

        protected void OnDisable()
        {
            DisableReverb();
        }

        private void DisableReverb()
        {
            _overlapCount = 0;
            _snapshotsAudioData.SetReverbSnapshot(_reverbName, false);
        }
    }
}