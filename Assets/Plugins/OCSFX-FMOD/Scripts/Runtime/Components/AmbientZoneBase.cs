using System;
using OCSFX.Attributes;
using OCSFX.FMOD.AudioData;
using OCSFX.Utility;
using UnityEngine;

namespace OCSFX.FMOD.Components
{
    public abstract class AmbientZoneBase: MonoBehaviour
    {
        [SerializeField] protected AmbienceAudioDataSO _ambienceAudioData;
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

        private bool AudioDataIsValid(AudioDataSO audioData)
        {
            if (audioData) return true;
            Debug.LogError($"{this} requires a reference to an {audioData.GetType()} to work.");
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