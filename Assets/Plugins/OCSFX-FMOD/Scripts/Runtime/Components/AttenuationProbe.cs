using System;
using FMODUnity;
using OCSFX.Utility.Debug;
using UnityEngine;
using UnityEngine.Serialization;

namespace OCSFX.FMOD.Components
{
    [AddComponentMenu(OCSFXAudioStatics.CREATE_COMPONENT_MENU_BASE + nameof(AttenuationProbe))]
    public class AttenuationProbe : PositionLerpBehavior<StudioListener>
    {
        private void OnEnable()
        {
            if (!GetListener()) return;
            
            SetAsListenerAttenuationObject(true);
        }

        private void OnDisable()
        {
            if (!GetListener()) return;
            
            SetAsListenerAttenuationObject(false);
        }
        
        private StudioListener GetListener()
        {
            if (Source) return Source;
#if UNITY_6000_0_OR_NEWER
            Source = FindFirstObjectByType<StudioListener>();
#else
            Listener = FindObjectOfType<StudioListener>();
#endif
            if (Source) return Source;
            
            OCSFXLogger.LogWarning($"No {nameof(StudioListener)} found in the scene. Adding one to the main camera.", this, _showDebug);
                
            var mainCamera = Camera.main;
            if (!mainCamera)
            {
                OCSFXLogger.LogError($"No main camera found in the scene! Unable to add {nameof(StudioListener)} to a camera.", this, _showDebug);
                return null;
            }
                
            Source = mainCamera.gameObject.AddComponent<StudioListener>();

            return Source;
        }
        
        private void SetAsListenerAttenuationObject(bool set)
        {
            if (!GetListener()) return;
            
            if (set)
            {
                Source.SetAttenuationObject(gameObject);
            }
            else if (Source.AttenuationObject == gameObject)
            {
                Source.ClearAttenuationObject();
            }
        }
    }
}