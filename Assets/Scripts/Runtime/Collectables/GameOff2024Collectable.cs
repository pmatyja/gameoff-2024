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
        
        [field: SerializeField] public UnityEvent OnCollect { get; protected set; }

        protected override void OnSpawn()
        {
            if (_data) _data.OnSpawn(transform, ID);
        }

        protected override void OnPickUp(GameObject pickerObject)
        {
            OnCollect?.Invoke();
            
            if (_data) _data.OnCollect(transform);
            
            base.OnPickUp(pickerObject);
        }

        private void OnValidate()
        {
            if (!_uniqueIDComponent) _uniqueIDComponent = GetComponent<GameOff2024UniqueID>();
        }

        private void Reset()
        {
            if (!_uniqueIDComponent) _uniqueIDComponent = GetComponent<GameOff2024UniqueID>();
        }
    }
}