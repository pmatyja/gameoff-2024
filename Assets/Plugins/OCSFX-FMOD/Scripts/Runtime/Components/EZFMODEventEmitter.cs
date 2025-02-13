using FMODUnity;
using OCSFX.EZFMOD;
using UnityEngine;

namespace OCSFX.EZFMOD.Components
{
    /// <summary>
    /// Wrapper class derived from FMOD StudioEventEmitter which will wait for initial banks to be loaded
    /// before attempting to play its associated Event.
    /// </summary>
    
    [AddComponentMenu(EZFMODRuntimeStatics.CREATE_COMPONENT_MENU_BASE + nameof(EZFMODEventEmitter))]
    public class EZFMODEventEmitter : StudioEventEmitter
    {
        protected override void HandleGameEvent(EmitterGameEvent gameEvent)
        {
            if (!EZFMODRuntimeStatics.MasterBanksLoaded)
                EZFMODRuntimeStatics.OnMasterBanksLoaded += () => base.HandleGameEvent(gameEvent);
            
            else base.HandleGameEvent(gameEvent);
        }
    }
}
