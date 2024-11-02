using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using OCSFX.FMOD.Types;
using OCSFX.Utility.Debug;

namespace OCSFX.FMOD.AudioData
{
    [CreateAssetMenu(menuName = _CREATE_ASSET_MENU_BASE + "Dialogue", fileName = nameof(DialogueAudioDataSO))]
    public class DialogueAudioDataSO : AudioDataSO
    {
        [Header("Dialogue Events")]
        [SerializeField] private List<FMODEvent> _events = new List<FMODEvent>();

        public void DialogueEventPlay(string eventName)
        {
            if (!TryGetDialogueEvent(eventName, out var foundEvent))
            {
                OCSFXLogger.LogWarning($"{this}: {eventName} was not found in Events.", this, _showDebug);
                return;
            }

            foundEvent.Play2D();
        }
        
        public void DialogueEventStop(string eventName)
        {
            if (!TryGetDialogueEvent(eventName, out var foundEvent))
            {
                OCSFXLogger.LogWarning($"{this}: {eventName} was not found in Events.", this, _showDebug);
                return;
            }

            foundEvent.Stop2D();
        }
        
        public EventReference GetDialogueEventByIndex(int index)
        {
            return _events[index].EventRef;
        }

        public EventReference GetDialogueEvent(string lineName)
        {
            return _events.GetEventReference(lineName);
        }
        
        public bool TryGetDialogueEvent(string lineName, out EventReference eventRef)
        {
            return _events.TryGetEventReference(lineName, out eventRef);
        }
    }
}