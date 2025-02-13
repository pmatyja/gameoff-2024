using System.Collections.Generic;
using FMODUnity;
using OCSFX.EZFMOD.Types;
using OCSFX.EZFMOD.Debug;
using OCSFX.EZFMOD.Utility.Generics;
using UnityEngine;

namespace OCSFX.EZFMOD.ScriptableObjects
{
    [CreateAssetMenu(menuName = _CREATE_ASSET_MENU_BASE + "Dialogue", fileName = nameof(EZFMODDialogueAudioDataSO))]
    public class EZFMODDialogueAudioDataSO : EZFMODAudioDataSO
    {
        [Header("Dialogue Events")]
        [SerializeField] private List<SerializedKeyValuePair<string, EZFMODEvent>> _events = new ();

        public void DialogueEventPlay(string eventName)
        {
            if (!TryGetDialogueEventByName(eventName, out var foundEvent))
            {
                OCSFXLogger.LogWarning($"{this}: {eventName} was not found in Events.", this, _showDebug);
                return;
            }

            foundEvent.Play2D();
        }
        
        public void DialogueEventStop(string eventName)
        {
            if (!TryGetDialogueEventByName(eventName, out var foundEvent))
            {
                OCSFXLogger.LogWarning($"{this}: {eventName} was not found in Events.", this, _showDebug);
                return;
            }

            foundEvent.StopAll(true);
        }
        
        public EZFMODEvent GetDialogueEventByIndex(int index)
        {
            return _events[index].Value;
        }

        public EZFMODEvent GetDialogueEvent(string lineName)
        {
            return GetDialogueEventByName(lineName);
        }
        
        private bool TryGetDialogueEventByName(string lineName, out EZFMODEvent foundEvent)
        {
            foundEvent = GetDialogueEventByName(lineName);
            return foundEvent;
        }
        
        private EZFMODEvent GetDialogueEventByName(string lineName)
        {
            return _events.Find(dialogueEvent => dialogueEvent.Key == lineName)?.Value;
        }
    }
}