using System;
using UnityEngine;

namespace GameOff2024.Utility
{
    public class ObservableCollider : MonoBehaviour
    {
        private Collider _collider;
        
        public event Action<Collider> OnCollisionEnterEvent;
        public event Action<Collider> OnCollisionStayEvent;
        public event Action<Collider> OnCollisionExitEvent;
        
        public event Action<Collider> OnTriggerEnterEvent;
        public event Action<Collider> OnTriggerStayEvent;
        public event Action<Collider> OnTriggerExitEvent;

        private void OnCollisionEnter(UnityEngine.Collision other)
        {
            OnCollisionEnterEvent?.Invoke(other.collider);
        }
        
        private void OnCollisionStay(UnityEngine.Collision other)
        {
            OnCollisionStayEvent?.Invoke(other.collider);
        }
        
        private void OnCollisionExit(UnityEngine.Collision other)
        {
            OnCollisionExitEvent?.Invoke(other.collider);
        }

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterEvent?.Invoke(other);
        }
        
        private void OnTriggerStay(Collider other)
        {
            OnTriggerStayEvent?.Invoke(other);
        }
        
        private void OnTriggerExit(Collider other)
        {
            OnTriggerExitEvent?.Invoke(other);
        }

        private void Reset()
        {
            _collider = GetComponent<Collider>();
            
            if (!_collider)
            {
                Debug.LogWarning("ObservableCollider requires a Collider component to function.", this);
            }
        }
    }
}