using UnityEngine;

namespace OCSFX.FMOD.Components
{
    /// <summary>
    /// Uses any attached Collider components to create a composite ambient volume.
    /// The ambience is achieved by creating an appropriate 2d event in FMOD and
    /// adding that event to the referenced AmbienceAudioData ScriptableObject.
    /// The amb event name here must match a name in the AmbienceAudioData.AmbEvents.
    /// </summary>
    
    [AddComponentMenu(OCSFXAudioStatics.CREATE_COMPONENT_MENU_BASE + nameof(AmbientZone))]
    public class AmbientZone : AmbientZoneBase
    {
        private void OnTriggerEnter(Collider other)
        {
            HandleTriggerEnter(other);
        }

        private void OnTriggerExit(Collider other)
        {
            HandleTriggerExit(other);
        }

        private void OnValidate()
        {
            var colliders = GetComponents<Collider>();

            if (colliders.Length <= 0) return;

            foreach (var colliderComp in colliders)
            {
                colliderComp.isTrigger = true;
            }
        }
    }   
}