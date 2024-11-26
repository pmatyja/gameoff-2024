using System;
using OCSFX.FMOD;
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
        [field: SerializeField] public UnityEvent<Transform> OnKeyInserted { get; private set; }
        
        [field: SerializeField] public bool CanInteract { get; private set; }
        [field: SerializeField] public bool IsRepeatable { get; private set; }
        [field: SerializeField] public bool HasInteracted { get; private set; }

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
            foreach (var keySlot in _keySlots)
            {
                var existingChildCollectables = keySlot.SlotTransform.GetComponentsInChildren<Collectable>();
                foreach (var collectable in existingChildCollectables)
                {
                    Destroy(collectable.gameObject);
                }
                
                keySlot.IsOccupied = false;
                
                var nonInteractableCollectable = Instantiate(keySlot.ExpectedKey.NonInteractablePrefab, keySlot.SlotTransform);
                nonInteractableCollectable.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                
                nonInteractableCollectable.SetActive(false);

                if (keySlot.ExpectedKey.OnUseKeySfx.IsNull) return;
                
                OnKeyInserted.AddListener(slotTransform =>
                {
                    keySlot.ExpectedKey.OnUseKeySfx.Play(slotTransform.gameObject);
                });
            }
        }

        public void Interact()
        {
            foreach (var keySlot in _keySlots)
            {
                if (keySlot.IsOccupied) continue;
                
                if (!ItemInventory.Instance.ContainsData(keySlot.ExpectedKey)) continue;
                
                keySlot.GetKey().SetActive(true);
                keySlot.IsOccupied = true;
                
                HasInteracted = true;
                
                OnKeyInserted?.Invoke(keySlot.SlotTransform);
                
                break;
            }

            if (!AllKeysInserted()) return;
            
            _blockMoverScript.Activate();
            CanInteract = false;
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
            if (!_interactionPrompt) return;
            
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