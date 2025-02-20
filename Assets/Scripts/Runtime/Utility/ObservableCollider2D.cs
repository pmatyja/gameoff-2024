using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Utility
{
    public class ObservableCollider2D : MonoBehaviour
    {
        [Header("Filters")]
        [Tooltip("The layer mask to filter collisions.")]
        public LayerMask CollisionLayerMask = 1;
        
        [Tooltip("If empty, all tags are accepted."), Tag]
        public List<string> CollisionTagFilter;
        
        [Header("Debug")]
        [SerializeField] private bool _showDebug;
        
        [field: Header("Events")]
        [field: SerializeField] public Trigger2DUnityEvents TriggerEvents { get; private set; }
        [field: SerializeField] public Collision2DUnityEvents CollisionEvents { get; private set; }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!IsCollisionObjectValid(other.gameObject)) return;
            CollisionEvents.OnCollisionEnterEvent?.Invoke(other);
            
            PrintDebug($"{nameof(OnCollisionEnter2D)}: {other.gameObject.name}", _showDebug);
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (!IsCollisionObjectValid(other.gameObject)) return;
            CollisionEvents.OnCollisionExitEvent?.Invoke(other);
            
            PrintDebug($"{nameof(OnCollisionExit2D)}: {other.gameObject.name}", _showDebug);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsCollisionObjectValid(other.gameObject)) return;
            TriggerEvents.OnTriggerEnterEvent?.Invoke(other);
            
            PrintDebug($"{nameof(OnTriggerEnter2D)}: {other.gameObject.name}", _showDebug);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!IsCollisionObjectValid(other.gameObject)) return;
            TriggerEvents.OnTriggerExitEvent?.Invoke(other);
            
            PrintDebug($"{nameof(OnTriggerExit2D)}: {other.gameObject.name}", _showDebug);
        }
        
        private bool IsCollisionObjectValid(GameObject other)
        {
            if (!other) return false;
            if (CollisionLayerMask == 0) return false;
            if (!((CollisionLayerMask & (1 << other.layer)) > 0)) return false; 
            if (CollisionTagFilter is not { Count: > 0 }) return true;

            return CollisionTagFilter.Count <= 0 || CollisionTagFilter.Contains(other.tag);
        }

        private void Reset()
        {
            if (TryGetComponent<Collider2D>(out _)) return;
            
            Debug.LogWarning($"{nameof(ObservableCollider)} requires a {nameof(Collider2D)} component to function." +
                             $"\n Adding a {nameof(BoxCollider2D)} as default.", this);
                
            gameObject.AddComponent<BoxCollider2D>();
        }
        
        private void PrintDebug(string message, bool condition)
        {
            if (!condition) return;
            
            Debug.Log(message, this);
        }
        
        [Serializable]
        public class Trigger2DUnityEvents
        {
            [field: SerializeField] public UnityEvent<Collider2D> OnTriggerEnterEvent {get; private set; }
            [field: SerializeField] public UnityEvent<Collider2D> OnTriggerExitEvent {get; private set; }
        }
        
        [Serializable]
        public class Collision2DUnityEvents
        {
            [field: SerializeField] public UnityEvent<Collision2D> OnCollisionEnterEvent { get; private set; }
            [field: SerializeField] public UnityEvent<Collision2D> OnCollisionExitEvent {get; private set; }
        }
    }
}