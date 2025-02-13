using OCSFX.EZFMOD;
using OCSFX.EZFMOD.Utility;
using OCSFX.EZFMOD.Debug;
using UnityEngine;

namespace OCSFX.EZFMOD.Components
{
    /// <summary>
    /// Uses any attached Collider components to create a composite ambient volume.
    /// The ambience is achieved by creating an appropriate 2d event in FMOD and
    /// adding that event to the referenced AmbienceAudioData ScriptableObject.
    /// The amb event name here must match a name in the AmbienceAudioData.AmbEvents.
    /// </summary>
    
    [AddComponentMenu(EZFMODRuntimeStatics.CREATE_COMPONENT_MENU_BASE + nameof(EZFMODAmbientZone))]
    public class EZFMODAmbientZone : EZFMODAmbientZoneBase
    {
        private Collider[] colliders;
        
        [SerializeField] private Color _gizmoFillColor;
        [SerializeField] private Color _gizmoWireColor;
        
        private readonly ColliderGizmo _colliderGizmo = new ColliderGizmo();
        
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
            ValidateColliders();
        }

        protected override void Reset()
        {
            base.Reset();
            
            if (!TryGetComponent<Collider>(out _))
            {
                gameObject.AddComponent<BoxCollider>();
            }

            ColorUtility.TryParseHtmlString("#0093A464", out _gizmoFillColor);
            ColorUtility.TryParseHtmlString("#0000FF80", out _gizmoWireColor);
        }

        private void ValidateColliders()
        {
            colliders = GetComponents<Collider>();

            if (colliders.Length == 0)
            {
                OCSFXLogger.LogWarning(
                    $"No {nameof(Collider)} components found on {name}." +
                    $"{nameof(EZFMODAmbientZone)} requires Trigger {nameof(Collider)}s to work.", this);
                
                gameObject.AddComponent<BoxCollider>();
            }

            foreach (var colliderComp in colliders)
            {
                colliderComp.isTrigger = true;
            }
        }

        private void OnDrawGizmos()
        {
            DrawColliderGizmos(ColliderGizmoBase.GizmoDrawComponent.Wire);
        }
        
        private void OnDrawGizmosSelected()
        {
            ValidateColliders();
            
            DrawColliderGizmos(ColliderGizmoBase.GizmoDrawComponent.Fill);
        }
        
        private void DrawColliderGizmos(ColliderGizmoBase.GizmoDrawComponent drawComponent)
        {
            _colliderGizmo.FillColor = _gizmoFillColor;
            _colliderGizmo.WireColor = _gizmoWireColor;
            
            _colliderGizmo.Draw(colliders, drawComponent);
        }
    }   
}
