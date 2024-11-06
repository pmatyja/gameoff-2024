using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Utility
{
    public class ObservableCollider : MonoBehaviour
    {
        [Header("Filters")]
        [Tooltip("The layer mask to filter collisions.")]
        public LayerMask CollisionLayerMask = 1;
        
        [Tooltip("If empty, all tags are accepted.")]
        public List<string> CollisionTagFilter;
        
        [field: Header("Events")]
        [field: SerializeField] public TriggerUnityEvents TriggerEvents { get; private set; }
        [field: SerializeField] public CollisionUnityEvents CollisionEvents { get; private set; }
        
        private Collider _collider;

        private void OnCollisionEnter(Collision other)
        {
            if (!IsCollisionObjectValid(other.gameObject)) return;
            CollisionEvents.OnCollisionEnterEvent?.Invoke(other);
        }

        private void OnCollisionStay(Collision other)
        {
            if (!IsCollisionObjectValid(other.gameObject)) return;
            CollisionEvents.OnCollisionStayEvent?.Invoke(other);
        }

        private void OnCollisionExit(Collision other)
        {
            if (!IsCollisionObjectValid(other.gameObject)) return;
            CollisionEvents.OnCollisionExitEvent?.Invoke(other);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsCollisionObjectValid(other.gameObject)) return;
            TriggerEvents.OnTriggerEnterEvent?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!IsCollisionObjectValid(other.gameObject)) return;
            TriggerEvents.OnTriggerStayEvent?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsCollisionObjectValid(other.gameObject)) return;
            TriggerEvents.OnTriggerExitEvent?.Invoke(other);
        }
        
        private bool IsCollisionObjectValid(GameObject other)
        {
            if (!other) return false;
            if (!((CollisionLayerMask & (1 << other.layer)) > 0)) return false; 
            if (CollisionTagFilter is not { Count: > 0 }) return true;

            return CollisionTagFilter.Count <= 0 || CollisionTagFilter.Contains(other.tag);
        }

        private void Reset()
        {
            if (TryGetComponent(out _collider)) return;
            
            Debug.LogWarning($"{nameof(ObservableCollider)} requires a {nameof(Collider)} component to function." +
                             $"\n Adding a {nameof(SphereCollider)} as default.", this);
                
            _collider = gameObject.AddComponent<SphereCollider>();
        }
        
        [Serializable]
        public class TriggerUnityEvents
        {
            [field: SerializeField] public UnityEvent<Collider> OnTriggerEnterEvent {get; private set; }
            [field: SerializeField] public UnityEvent<Collider> OnTriggerStayEvent {get; private set; }
            [field: SerializeField] public UnityEvent<Collider> OnTriggerExitEvent {get; private set; }
        }
        
        [Serializable]
        public class CollisionUnityEvents
        {
            [field: SerializeField] public UnityEvent<Collision> OnCollisionEnterEvent { get; private set; }
            [field: SerializeField] public UnityEvent<Collision> OnCollisionStayEvent {get; private set; }
            [field: SerializeField] public UnityEvent<Collision> OnCollisionExitEvent {get; private set; }
        }
    }
}