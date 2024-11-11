using System;
using UnityEngine;

namespace Runtime.Utility
{
    public abstract class TriggerOnMonoBehaviourFunction : MonoBehaviour
    {
        public MonoBehaviourFunction TriggerOn;
        
        protected abstract void Trigger();
        
        private void TriggerInternal(MonoBehaviourFunction triggerType)
        {
            // Check the bitmask if the trigger type is enabled
            if ((TriggerOn & triggerType) == 0) return;
            
            Trigger();
        }
        
        private void Awake() => TriggerInternal(MonoBehaviourFunction.Awake);
        private void Start() => TriggerInternal(MonoBehaviourFunction.Start);
        private void OnEnable() => TriggerInternal(MonoBehaviourFunction.OnEnable);
        private void OnDisable() => TriggerInternal(MonoBehaviourFunction.OnDisable);
        private void OnDestroy() => TriggerInternal(MonoBehaviourFunction.OnDestroy);

        private void OnTriggerEnter(Collider other) => TriggerInternal(MonoBehaviourFunction.OnTriggerEnter);
        private void OnTriggerExit(Collider other) => TriggerInternal(MonoBehaviourFunction.OnTriggerExit);
        
        private void OnCollisionEnter(Collision other) => TriggerInternal(MonoBehaviourFunction.OnCollisionEnter);
        private void OnCollisionExit(Collision other) => TriggerInternal(MonoBehaviourFunction.OnCollisionExit);
        
        private void OnTriggerEnter2D(Collider2D other) => TriggerInternal(MonoBehaviourFunction.OnTriggerEnter2D);
        private void OnTriggerExit2D(Collider2D other) => TriggerInternal(MonoBehaviourFunction.OnTriggerExit2D);
        
        private void OnCollisionEnter2D(Collision2D other) => TriggerInternal(MonoBehaviourFunction.OnCollisionEnter2D);
        private void OnCollisionExit2D(Collision2D other) => TriggerInternal(MonoBehaviourFunction.OnCollisionExit2D);
        
        private void OnMouseEnter() => TriggerInternal(MonoBehaviourFunction.OnMouseEnter);
        private void OnMouseExit() => TriggerInternal(MonoBehaviourFunction.OnMouseExit);
        private void OnMouseOver() => TriggerInternal(MonoBehaviourFunction.OnMouseOver);
        private void OnMouseDown() => TriggerInternal(MonoBehaviourFunction.OnMouseDown);
        private void OnMouseUp() => TriggerInternal(MonoBehaviourFunction.OnMouseUp);
    }
}