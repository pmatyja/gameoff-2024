using OCSFX.FMOD.AudioData;
using OCSFX.Utility.Debug;
using UnityEngine;

namespace OCSFX.FMOD.Components
{
    [AddComponentMenu(OCSFXAudioStatics.CREATE_COMPONENT_MENU_BASE + nameof(AnimationEventHandler))]
    public class AnimationEventHandler : MonoBehaviour
    {
        [SerializeField] private AnimationAudioDataSO _audioData;
        [SerializeField] private GameObject _soundSource;

        private void Awake()
        {
            if (!_soundSource) _soundSource = gameObject;
        }

        public void PlaySoundEvent(string eventName)
        {
            if (!_audioData.TryGetAnimEvent(eventName, out var fmodEventRef))
            {
                OCSFXLogger.LogError(
                    $"'{eventName}' was not found in {this}'s {nameof(AnimationAudioDataSO)} events.", this);
                return;
            }

            fmodEventRef.Play(_soundSource);
        }

        public void PlaySoundEvent(string eventName, string parameter, float parameterValue)
        {
            if (!_audioData.TryGetAnimEvent(eventName, out var fmodEventRef))
            {
                OCSFXLogger.LogError(
                    $"'{eventName}' was not found in {this}'s {nameof(AnimationAudioDataSO)} events.", this);
                return;
            }

            fmodEventRef.Play(_soundSource, parameter, parameterValue);
        }

        public void StopSoundEvent(string eventName)
        {
            if (!_audioData.TryGetAnimEvent(eventName, out var fmodEventRef))
            {
                OCSFXLogger.LogError(
                    $"'{eventName}' was not found in {this}'s {nameof(AnimationAudioDataSO)} events.", this);
                return;
            }

            fmodEventRef.Stop(_soundSource);
        }

        private void OnValidate()
        {
            if (!_soundSource) _soundSource = gameObject;
        }
    }
}