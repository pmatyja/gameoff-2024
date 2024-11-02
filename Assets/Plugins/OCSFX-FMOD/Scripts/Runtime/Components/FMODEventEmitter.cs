using FMODUnity;
using UnityEngine;

namespace OCSFX.FMOD.Components
{
    /// <summary>
    /// Wrapper class derived from FMOD StudioEventEmitter which will wait for initial banks to be loaded
    /// before attempting to play its associated Event.
    /// </summary>
    
    [AddComponentMenu(OCSFXAudioStatics.CREATE_COMPONENT_MENU_BASE + nameof(FMODEventEmitter))]
    public class FMODEventEmitter : StudioEventEmitter
    {
        protected override void HandleGameEvent(EmitterGameEvent gameEvent)
        {
            if (!OCSFXAudioStatics.StartupBanksAreLoaded)
                OCSFXAudioStatics.StartupBanksLoaded += () => base.HandleGameEvent(gameEvent);
            
            else base.HandleGameEvent(gameEvent);
        }
    }
}
