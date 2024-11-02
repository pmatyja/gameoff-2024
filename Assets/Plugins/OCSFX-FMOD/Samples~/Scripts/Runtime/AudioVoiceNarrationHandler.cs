using FMOD.Studio;
using FMODUnity;
using OCSFX.FMOD.AudioData;
using OCSFX.FMOD.Utility;
using UnityEngine;

namespace OCSFX.FMOD
{
    public class AudioVoiceNarrationHandler: MonoBehaviour
    {
        [SerializeField] private DialogueAudioDataSO _dialogueAudioData;
        [SerializeField] private SnapshotsAudioDataSO _snapshotsAudioData;
        [SerializeField] private string _dialogSnapshotName;

        private EventInstance _currentNarrationInstance;
        private EventReference _currentNarrationEvent;

        public void BeginNarration(EventReference narratorVoiceEvent)
        {
            _snapshotsAudioData.SetStateSnapshot(_dialogSnapshotName, true);
            _currentNarrationEvent = narratorVoiceEvent;
            _currentNarrationInstance = _currentNarrationEvent.Play(gameObject);
        }
        
        public void KillNarration()
        {
            _snapshotsAudioData.SetStateSnapshot(_dialogSnapshotName, false);
            _currentNarrationInstance.Stop();
        }
    }
}