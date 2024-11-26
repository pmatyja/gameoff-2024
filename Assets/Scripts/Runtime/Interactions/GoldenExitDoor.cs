using System;
using System.Collections.Generic;
using Runtime.Collectables;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Interactions
{
    public class GoldenExitDoor : MonoBehaviour, ICharacterInteractable
    {
        [SerializeField] private DoorKeySlot[] _keySlots;
        [SerializeField] private BlockMoverScript _blockMoverScript;
        [SerializeField] private InteractionPrompt _interactionPrompt;
        
        [field: Space]
        [field: SerializeField] public UnityEvent<CollectableData> OnKeyInsertedEvent { get; private set; }
        [field: SerializeField] public UnityEvent OnAllKeysInsertedEvent { get; private set; }
        
        [field: SerializeField] public bool CanInteract { get; private set; }
        [field: SerializeField] public bool IsRepeatable { get; private set; }
        [field: SerializeField] public bool HasInteracted { get; private set; }

        private readonly List<CollectableData> _vacantKeys = new List<CollectableData>();

        private void OnEnable()
        {
            ItemInventory.OnKeyItemsCollectedChanged += OnKeyItemsCollectedChanged;
        }

        private void OnDisable()
        {
            ItemInventory.OnKeyItemsCollectedChanged -= OnKeyItemsCollectedChanged;
        }

        private void OnKeyItemsCollectedChanged(int count)
        {
            CanInteract = count > 0;
        }

        private void Start()
        {
            InitializeSlots();
            IsRepeatable = true;
        }

        public void InitializeSlots()
        {
            _vacantKeys.Clear();

            foreach (var keySlot in _keySlots)
            {
                keySlot.CreateKeyInstance();
                _vacantKeys.Add(keySlot.ExpectedKey);
            }
        }

        public void Interact()
        {
            foreach (var keySlot in _keySlots)
            {
                if (keySlot.IsOccupied) continue;
                
                if (!ItemInventory.Instance.ContainsData(keySlot.ExpectedKey)) continue;
                
                InsertKey(keySlot);
                break;
            }

            if (!AllKeysInserted()) return;
            
            _blockMoverScript.Activate();
            CanInteract = false;
            
            OnAllKeysInsertedEvent?.Invoke();
        }

        private void InsertKey(DoorKeySlot keySlot)
        {
            keySlot.KeyInstance.SetActive(true);
            keySlot.ExpectedKey.OnUseKey(keySlot.KeyInstance.transform);
            _vacantKeys.Remove(keySlot.ExpectedKey);
            
            HasInteracted = true;
            OnKeyInsertedEvent?.Invoke(keySlot.ExpectedKey);
            
            CanInteract = ItemInventory.Instance.ContainsAny(_vacantKeys);
        }

        public bool AllKeysInserted()
        {
            foreach (var keySlot in _keySlots)
            {
                if (!keySlot.IsOccupied) return false;
            }

            return true;
        }

        public void ShowInteractionPrompt(bool show)
        {
            if (!_interactionPrompt || _interactionPrompt.IsShowing == show) return;
            
            _interactionPrompt.Show(show);
        }
        
        [Serializable]
        private class DoorKeySlot
        {
            public Transform SlotTransform;
            public CollectableData ExpectedKey;
            public bool IsOccupied => _keyInstance && _keyInstance.activeSelf;
            public GameObject KeyInstance => _keyInstance;

            private GameObject _keyInstance;
            
            public void CreateKeyInstance()
            {
                if (_keyInstance)
                {
                    Destroy(_keyInstance);
                }
                
                _keyInstance = Instantiate(ExpectedKey.NonInteractablePrefab, SlotTransform);
                _keyInstance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                _keyInstance.SetActive(false);
            }
        }
    }
}