using FMODUnity;
using OCSFX.EZFMOD.Debug;
using OCSFX.EZFMOD.Utility;

namespace OCSFX.EZFMOD.Components
{
    public class EZFMODListenerAttenuationProbe : BoomArm<StudioListener>
    {
        private void Start()
        {
            if (!Source) Source = FindFirstObjectByType<StudioListener>();
        
            if (!Source)
            {
                OCSFXLogger.LogError($"No {nameof(StudioListener)} found in scene. " +
                                     $"{nameof(EZFMODListenerAttenuationProbe)} will not work.", this);
                return;
            }
        
            if (!Target)
            {
                OCSFXLogger.LogError($"{nameof(EZFMODListenerAttenuationProbe)} will not work without a Target transform.");
                return;
            }
            
            Source.SetAttenuationObject(gameObject);
        }
    }
}