using System;
using System.Collections.Generic;
using OCSFX.EZFMOD.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Utility
{
    public class ObservableCollider : MonoBehaviour
    {
        [Header("Filters")]
        [Tooltip("The layer mask to filter collisions.")]
        public LayerMask CollisionLayerMask = 1;
        
        [Tooltip("If empty, all tags are accepted."), Tag]
        public List<string> CollisionTagFilter;
        
        [Header("Debug")]
        [SerializeField] private bool _showDebug;
        
        [field: Header("Events")]
        [field: SerializeField] public TriggerUnityEvents TriggerEvents { get; private set; }
        [field: SerializeField] public CollisionUnityEvents CollisionEvents { get; private set; }

        private void OnCollisionEnter(Collision other)
        {
            if (!IsCollisionObjectValid(other.gameObject)) return;
            CollisionEvents.OnCollisionEnterEvent?.Invoke(other);
            
            PrintDebug($"{nameof(OnCollisionEnter)}: {other.gameObject.name}", _showDebug);
        }

        private void OnCollisionExit(Collision other)
        {
            if (!IsCollisionObjectValid(other.gameObject)) return;
            CollisionEvents.OnCollisionExitEvent?.Invoke(other);
            
            PrintDebug($"{nameof(OnCollisionExit)}: {other.gameObject.name}", _showDebug);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsCollisionObjectValid(other.gameObject)) return;
            TriggerEvents.OnTriggerEnterEvent?.Invoke(other);
            
            PrintDebug($"{nameof(OnTriggerEnter)}: {other.gameObject.name}", _showDebug);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsCollisionObjectValid(other.gameObject)) return;
            TriggerEvents.OnTriggerExitEvent?.Invoke(other);
            
            PrintDebug($"{nameof(OnTriggerExit)}: {other.gameObject.name}", _showDebug);
        }
        
        private bool IsCollisionObjectValid(GameObject other)
        {
            if (!other) return false;
            if (CollisionLayerMask == 0) return false;
            if (!CollisionLayerMask.ContainsLayer(other.layer)) return false;

            return CollisionTagFilter.Count <= 0 || CollisionTagFilter.Contains(other.tag);
        }

        private void Reset()
        {
            if (TryGetComponent<Collider>(out _)) return;
            
            Debug.LogWarning($"{nameof(ObservableCollider)} requires a {nameof(Collider)} component to function." +
                             $"\n Adding a {nameof(BoxCollider)} as default.", this);
                
            gameObject.AddComponent<BoxCollider>();
        }
        
        private void PrintDebug(string message, bool condition)
        {
            if (!condition) return;
            
            Debug.Log(message, this);
        }
        
        [Serializable]
        public class TriggerUnityEvents
        {
            [field: SerializeField] public UnityEvent<Collider> OnTriggerEnterEvent {get; private set; }
            [field: SerializeField] public UnityEvent<Collider> OnTriggerExitEvent {get; private set; }
        }
        
        [Serializable]
        public class CollisionUnityEvents
        {
            [field: SerializeField] public UnityEvent<Collision> OnCollisionEnterEvent { get; private set; }
            [field: SerializeField] public UnityEvent<Collision> OnCollisionExitEvent {get; private set; }
        }
    }
}