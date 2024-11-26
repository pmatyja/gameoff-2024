using System;
using System.Linq;
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

        private CollectableData[] _allKeyDatas;

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
            _allKeyDatas = new CollectableData[_keySlots.Length];
            
            for (int i = 0; i < _keySlots.Length; i++)
            {
                var keySlot = _keySlots[i];
                for (int j = 0; j < keySlot.SlotTransform.childCount; j++)
                {
                    Destroy(keySlot.SlotTransform.GetChild(j).gameObject);
                }
    
                keySlot.IsOccupied = false;
    
                var nonInteractableCollectable = Instantiate(keySlot.ExpectedKey.NonInteractablePrefab, keySlot.SlotTransform);
                nonInteractableCollectable.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    
                nonInteractableCollectable.SetActive(false);
                
                _allKeyDatas[i] = keySlot.ExpectedKey;
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
            keySlot.GetKey().SetActive(true);
            keySlot.IsOccupied = true;
            keySlot.ExpectedKey.OnUseKey(keySlot.GetKey().transform);
            
            HasInteracted = true;
            OnKeyInsertedEvent?.Invoke(keySlot.ExpectedKey);
            
            UpdateCanInteract();
        }

        private void UpdateCanInteract()
        {
            // If the door doesn't have any unoccupied slots which correspond with key items in the inventory,
            // we can't interact.
            var unInsertedKeyDatas = _keySlots
                .Where(slot => !slot.IsOccupied)
                .Select(slot => slot.ExpectedKey);
            
            CanInteract = ItemInventory.Instance.ContainsAny(unInsertedKeyDatas);
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
            public bool IsOccupied;
            
            public GameObject GetKey() => SlotTransform.GetChild(0).gameObject;
        }
    }
}