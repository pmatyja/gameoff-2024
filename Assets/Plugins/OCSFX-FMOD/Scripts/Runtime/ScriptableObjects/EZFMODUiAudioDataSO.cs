using System.Collections.Generic;
using OCSFX.EZFMOD.Types;
using OCSFX.EZFMOD.Debug;
using OCSFX.EZFMOD.Utility.Generics;
using UnityEngine;

namespace OCSFX.EZFMOD.ScriptableObjects
{
    [CreateAssetMenu(menuName = _CREATE_ASSET_MENU_BASE + "UI", fileName = nameof(EZFMODUiAudioDataSO))]
    public class EZFMODUiAudioDataSO : EZFMODAudioDataSO
    {
        [Header("UI Events")]
        [SerializeField] private List<SerializedKeyValuePair<string, EZFMODEvent>> _uiEvents = new()
        {
            new SerializedKeyValuePair<string, EZFMODEvent>("None", null),
            new SerializedKeyValuePair<string, EZFMODEvent>("PlayButtonPress", null),
            new SerializedKeyValuePair<string, EZFMODEvent>("QuitButtonPress", null),
            new SerializedKeyValuePair<string, EZFMODEvent>("BackButtonPress", null),
            new SerializedKeyValuePair<string, EZFMODEvent>("ButtonPress", null),
            new SerializedKeyValuePair<string, EZFMODEvent>("Focus", null),
            new SerializedKeyValuePair<string, EZFMODEvent>("Unfocus", null),
            new SerializedKeyValuePair<string, EZFMODEvent>("MenuOpen", null),
            new SerializedKeyValuePair<string, EZFMODEvent>("MenuClose", null),
            new SerializedKeyValuePair<string, EZFMODEvent>("PauseMenuOpen", null),
            new SerializedKeyValuePair<string, EZFMODEvent>("PauseMenuClose", null)
        };
        
        public void UiEventPlay(string uiEventName)
        {
            if (!TryGetUiEventByName(uiEventName, out var eventRef))
            {
                OCSFXLogger.LogWarning($"{uiEventName} was not found in {_uiEvents}. Check {this}.", this, _showDebug);
                return;
            }

            eventRef.PlayOneShot();;
        }
        
        public void UiEventStop(string uiEventName)
        {
            if (!TryGetUiEventByName(uiEventName, out var eventRef))
            {
                OCSFXLogger.LogWarning($"{uiEventName} was not found in {_uiEvents}. Check {this}.", this, _showDebug);
                return;
            }

            eventRef.StopAll(true);
        }
        
        private bool TryGetUiEventByName(string eventName, out EZFMODEvent eventRef)
        {
            eventRef = GetUiEventByName(eventName);

            return eventRef;
        }
        
        private EZFMODEvent GetUiEventByName(string uiEventName)
        {
            var uiEvent = _uiEvents.Find(entry => entry.Key == uiEventName)?.Value;

            return uiEvent;
        }
    }
}