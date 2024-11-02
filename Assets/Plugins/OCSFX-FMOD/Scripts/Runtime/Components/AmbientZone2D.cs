using UnityEngine;
using Debug = UnityEngine.Debug;

namespace OCSFX.FMOD.Components
{
    /// <summary>
    /// Uses any attached Collider components to create a composite ambient volume.
    /// The ambience is achieved by creating an appropriate 2d event in FMOD and
    /// adding that event to the referenced AmbienceAudioData ScriptableObject.
    /// The amb event name here must match a name in the AmbienceAudioData.AmbEvents.
    /// </summary>
    
    [AddComponentMenu(OCSFXAudioStatics.CREATE_COMPONENT_MENU_BASE + nameof(AmbientZone2D))]
    public class AmbientZone2D : AmbientZoneBase
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            HandleTriggerEnter(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            HandleTriggerExit(other);
        }

        private void OnValidate()
        {
            var colliders = GetComponents<Collider2D>();

            if (colliders.Length <= 0)
            {
                Debug.LogWarning($"{this} requires Collider2D(s) to work!");
                return;
            }

            foreach (var colliderComp in colliders)
            {
                colliderComp.isTrigger = true;
            }
        }
    }   
}
