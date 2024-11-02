using UnityEngine;

namespace OCSFX.FMOD.Components
{
    [AddComponentMenu(OCSFXAudioStatics.CREATE_COMPONENT_MENU_BASE + nameof(ReverbZone2D))]
    public class ReverbZone2D : ReverbZoneBase
    {
        /// <summary>
        /// Uses any attached Collider components to create a composite reverb volume.
        /// The reverb is achieved by creating an appropriate Snapshot in FMOD and
        /// adding that Snapshot to the referenced SnapshotsAudioData ScriptableObject.
        /// The reverb name here must match a name in the SnapshotsAudioData.Reverbs.
        /// </summary>

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
