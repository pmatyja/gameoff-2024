using System.Collections.Generic;
using FMODUnity;
using OCSFX.EZFMOD;
using OCSFX.EZFMOD.Types;
using OCSFX.EZFMOD.Debug;
using UnityEngine;

namespace OCSFX.EZFMOD.ScriptableObjects
{
    [CreateAssetMenu(menuName = _CREATE_ASSET_MENU_BASE + "UI", fileName = nameof(EZFMODUiAudioDataSO))]
    public class EZFMODUiAudioDataSO : EZFMODAudioDataSO
    {
        [Header("UI Events")]
        [SerializeField] private List<FMODEvent> _events = new List<FMODEvent>()
        {
            new FMODEvent("None", new EventReference()),
            new FMODEvent("PlayButtonPress", new EventReference()),
            new FMODEvent("QuitButtonPress", new EventReference()),
            new FMODEvent("BackButtonPress", new EventReference()),
            new FMODEvent("ButtonPress", new EventReference()),
            new FMODEvent("Focus", new EventReference()),
            new FMODEvent("Unfocus", new EventReference()),
            new FMODEvent("MenuOpen", new EventReference()),
            new FMODEvent("MenuClose", new EventReference()),
            new FMODEvent("PauseMenuOpen", new EventReference()),
            new FMODEvent("PauseMenuClose", new EventReference())
        };
        
        public void UiEventPlay(string uiEventName)
        {
            if (!_events.TryGetEventReference(uiEventName, out var eventRef))
            {
                OCSFXLogger.LogWarning($"{uiEventName} was not found in {_events}. Check {this}.", this, _showDebug);
                return;
            }

            eventRef.Play2D();
        }
        
        public void UiEventStop(string uiEventName)
        {
            if (!_events.TryGetEventReference(uiEventName, out var eventRef))
            {
                OCSFXLogger.LogWarning($"{uiEventName} was not found in {_events}. Check {this}.", this, _showDebug);
                return;
            }

            eventRef.Stop2D();
        }
    }
}