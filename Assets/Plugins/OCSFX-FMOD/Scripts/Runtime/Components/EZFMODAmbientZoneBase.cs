using OCSFX.EZFMOD.Attributes;
using OCSFX.EZFMOD.Debug;
using OCSFX.EZFMOD.Utility;
using OCSFX.EZFMOD.ScriptableObjects;
using UnityEngine;

namespace OCSFX.EZFMOD.Components
{
    public abstract class EZFMODAmbientZoneBase: MonoBehaviour
    {
        [SerializeField] protected EZFMODAmbienceAudioDataSO _ambienceAudioData;
        [SerializeField] protected LayerMask _layerMask;
        [SerializeField] protected string _ambEventName;
        [SerializeField] [Range(0f, 100f)] protected float _priority = 1.0f;
        
        [SerializeField, ReadOnly] private int _overlapCount;

        public string AmbEventName => _ambEventName;
        public float Priority => _priority;

        protected void HandleTriggerEnter(Component other)
        {
            if (!_layerMask.Contains(other)) return;
            
            // Don't include disabled game objects
            if (!other.gameObject.activeSelf) return;

            if (!AudioDataIsValid(_ambienceAudioData)) return;

            if (_overlapCount < 1)
            {
                _ambienceAudioData.OnAmbientZoneEntered(this);
            }
        
            _overlapCount++;
        }

        protected void HandleTriggerExit(Component other)
        {
            if (!_layerMask.Contains(other)) return;
            
            // Don't include disabled game objects
            if (!other.gameObject.activeSelf) return;

            if (!AudioDataIsValid(_ambienceAudioData)) return;
            
            _overlapCount--;

            if (_overlapCount >= 1) return;
            
            _ambienceAudioData.OnAmbientZoneExited(this);
        }

        private void DisableAmbientSound()
        {
            _overlapCount = 0;

            if (!AudioDataIsValid(_ambienceAudioData)) return;
            
            _ambienceAudioData.OnAmbientZoneExited(this);
        }

        private bool AudioDataIsValid(EZFMODAudioDataSO audioData)
        {
            if (audioData) return true;
            
            OCSFXLogger.LogWarning($"[{nameof(EZFMODAmbientZoneBase)}] requires a reference to a {nameof(EZFMODAmbienceAudioDataSO)} to work.", this);
            return false;
        }

        protected virtual void OnDisable()
        {
            DisableAmbientSound();
        }

        protected virtual void Reset()
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
    }
}