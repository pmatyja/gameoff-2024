using System;
using System.Collections;
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
        private static readonly int _BASE_COLOR_PROPERTY_ID = Shader.PropertyToID("_BaseColor");
        private static readonly int _EMISSION_COLOR_PROPERTY_ID = Shader.PropertyToID("_EmissionColor");
        
        [SerializeField] private DoorKeySlot[] _keySlots;
        [SerializeField] private InteractionPrompt _interactionPrompt;
        
        [Header("Star Color")]
        [SerializeField] private MeshRenderer _starRenderer;
        [SerializeField] private Material _starMaterial;
        [SerializeField, Readonly] private Material _starMaterialInstance;
        [SerializeField, Min(0)] private float _starInterpColorDuration = 1f;
        [Space]
        [SerializeField] private KeyColorCombination[] _keyColorCombinations;
        
        [field: Space]
        [field: SerializeField] public UnityEvent<CollectableData> OnKeyInsertedEvent { get; private set; }
        [field: SerializeField] public UnityEvent OnAllKeysInsertedEvent { get; private set; }
        
        [field: SerializeField] public bool CanInteract { get; private set; }
        [field: SerializeField] public bool IsRepeatable { get; private set; }
        [field: SerializeField] public bool HasInteracted { get; private set; }

        private readonly List<CollectableData> _vacantKeys = new List<CollectableData>();
        
        private Coroutine _interpStarColorCoroutine;

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
                InterpolateStarColor(
                    occupiedKeys[0].Material.GetColor(_BASE_COLOR_PROPERTY_ID),
                    occupiedKeys[0].Material.GetColor(_EMISSION_COLOR_PROPERTY_ID), 
                    _starInterpColorDuration);
                
            }
            else
            {
                InterpolateStarColor(
                    keyColorCombination.CombinedBaseColor, 
                    keyColorCombination.CombinedEmissionColor, 
                    _starInterpColorDuration);
            }
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
        
        private void SetStarColor(Color baseColor, Color emissionColor)
        {
            _starMaterialInstance.SetColor(_BASE_COLOR_PROPERTY_ID, baseColor);
            _starMaterialInstance.SetColor(_EMISSION_COLOR_PROPERTY_ID, emissionColor);
        }
        
        private void InterpolateStarColor(Color targetBaseColor, Color targetEmissionColor, float duration = 1f)
        {
            if (_interpStarColorCoroutine != null)
            {
                StopCoroutine(_interpStarColorCoroutine);
            }
            
            _interpStarColorCoroutine = StartCoroutine(Co_InterpolateStarColor
            (
                new Tuple<Color, Color>
                (
                    _starMaterialInstance.GetColor(_BASE_COLOR_PROPERTY_ID), 
                    _starMaterialInstance.GetColor(_EMISSION_COLOR_PROPERTY_ID)
                ),
                new Tuple<Color, Color>(targetBaseColor, targetEmissionColor),
                duration
            ));
        }

        private IEnumerator Co_InterpolateStarColor(
            Tuple<Color, Color> startBaseAndEmissionColor, 
            Tuple<Color, Color> targetBaseAndEmissionColor, 
            float duration = 1f)
        {
            var elapsedTime = 0f;
            var startBaseColor = startBaseAndEmissionColor.Item1;
            var startEmissionColor = startBaseAndEmissionColor.Item2;
            var targetBaseColor = targetBaseAndEmissionColor.Item1;
            var targetEmissionColor = targetBaseAndEmissionColor.Item2;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                var t = elapsedTime / duration;
                SetStarColor(Color.Lerp(startBaseColor, targetBaseColor, t), Color.Lerp(startEmissionColor, targetEmissionColor, t));
                yield return null;
            }
            
            SetStarColor(targetBaseColor, targetEmissionColor);
            
            _interpStarColorCoroutine = null;
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