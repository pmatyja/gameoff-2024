using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Collectables;
using Runtime.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Interactions
{
    public class GoldenExitDoor : MonoBehaviour, ICharacterInteractable
    {
        private static readonly int _baseColorPropertyID = Shader.PropertyToID("_BaseColor");
        private static readonly int _emissionColorPropertyID = Shader.PropertyToID("_EmissionColor");
        
        [SerializeField] private DoorKeySlot[] _keySlots;
        [SerializeField] private BlockMoverScript _blockMoverScript;
        [SerializeField] private InteractionPrompt _interactionPrompt;
        
        [Header("Star Color")]
        [SerializeField] private MeshRenderer _starRenderer;
        [SerializeField] private Material _starMaterial;
        [SerializeField, Readonly] private Material _starMaterialInstance;
        [Space]
        [SerializeField] private KeyColorCombination[] _keyColorCombinations;
        
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
            
            _starMaterialInstance = Instantiate(_starMaterial);
            _starRenderer.material = _starMaterialInstance;
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

            UpdateStarColor();
            
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

        private void UpdateStarColor()
        {
            var occupiedKeys = _keySlots.Where(slot => slot.IsOccupied).Select(slot => slot.ExpectedKey).ToArray();
            if (occupiedKeys.Length == 0) return;
            
            if (!TryGetKeyColorCombination(occupiedKeys, out var keyColorCombination))
            {
                // If no combination is found, use the first key's color
                SetStarColor
                (
                    occupiedKeys[0].Material.GetColor(_baseColorPropertyID), 
                    occupiedKeys[0].Material.GetColor(_emissionColorPropertyID)
                );
            }
            else
            {
                SetStarColor(keyColorCombination.CombinedBaseColor, keyColorCombination.CombinedEmissionColor);
            }
        }
        
        private void SetStarColor(Color baseColor, Color emissionColor)
        {
            _starMaterialInstance.SetColor(_baseColorPropertyID, baseColor);
            _starMaterialInstance.SetColor(_emissionColorPropertyID, emissionColor);
        }
        
        private bool TryGetKeyColorCombination(CollectableData[] combination, out KeyColorCombination result)
        {
            // Check that all keys in the requested combination are present in the keyColorCombination
            // They do not need to be in the same order, but they must have the same keys
            
            foreach (var keyColorCombination in _keyColorCombinations)
            {
                if (combination.Length != keyColorCombination.Combination.Length) continue;

                if (combination.All(key => keyColorCombination.Combination.Contains(key)))
                {
                    result = keyColorCombination;
                    return true;
                }
            }

            result = null;
            return false;
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