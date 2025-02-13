using OCSFX.EZFMOD;
using OCSFX.EZFMOD.Debug;
using UnityEngine;

namespace OCSFX.EZFMOD.Components
{
    [AddComponentMenu(EZFMODRuntimeStatics.CREATE_COMPONENT_MENU_BASE + nameof(EZFMODReverbZone))]
    public class EZFMODReverbZone : EZFMODReverbZoneBase
    {
        /// <summary>
        /// Uses any attached Collider components to create a composite reverb volume.
        /// The reverb is achieved by creating an appropriate Snapshot in FMOD and
        /// adding that Snapshot to the referenced SnapshotsAudioData ScriptableObject.
        /// The reverb name here must match a name in the SnapshotsAudioData.Reverbs.
        /// </summary>

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
            if (colliders.Length <= 0)
            {
                OCSFXLogger.LogWarning($"{this} requires Collider(s) to work!");
                return;
            }

            foreach (var colliderComp in colliders)
            {
                colliderComp.isTrigger = true;
            }
        }
        
        private void Reset()
        {
            if (!TryGetComponent<Collider>(out _))
            {
                gameObject.AddComponent<BoxCollider>();
            }
        }
    }   
}
