using System;
using Runtime.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Collectables
{
    // Derived from Lavgine Collectable
    [RequireComponent(typeof(GameOff2024UniqueID))]
    public class GameOff2024Collectable : Collectable
    {
        [SerializeField, Expandable] private CollectableData _data;
        [SerializeField] private GameOff2024UniqueID _uniqueIDComponent;
        
        public string ID => _uniqueIDComponent.ID;
        
        public CollectableData Data => _data;
        
        [field: SerializeField] public UnityEvent OnCollect { get; protected set; }

        protected override void OnSpawn()
        {
            if (!_data) return;
            
            TryApplyMaterial();
            _data.OnSpawn(transform, ID);
            
            if (!_data.IsUnique && !_data.IsTransient)
            {
                GameOff2024Statics.RegisterOptionalCollectable(this);
            }
        }

        protected override void OnPickUp(GameObject pickerObject)
        {
            OnCollect?.Invoke();
            
            if (_data) _data.OnCollect(transform);
            
            base.OnPickUp(pickerObject);
        }
        
        private bool TryApplyMaterial()
        {
            if (!_data) return false;
            
            if (!_data.Material) return false;
            
            var meshRenderer = GetComponentInChildren<Renderer>();
            if (!meshRenderer) return false;
            
            meshRenderer.material = _data.Material;
            return true;
        }

        private void OnValidate()
        {
            if (!_uniqueIDComponent) _uniqueIDComponent = GetComponent<GameOff2024UniqueID>();
            
            TryApplyMaterial();
        }

        private void Reset()
        {
            if (!_uniqueIDComponent) _uniqueIDComponent = GetComponent<GameOff2024UniqueID>();
            
            TryApplyMaterial();
        }
    }
}