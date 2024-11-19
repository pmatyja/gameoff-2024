using System.Collections.Generic;
using OCSFX.Utility.Debug;
using Runtime.Collectables;
using UnityEngine;

namespace Runtime.Interactions
{
    public class ExitDoor : InteractableDoor
    {
        [SerializeField] private List<CollectableData> _requiredCollectables = new List<CollectableData>(); 
        [Space]
        [SerializeField] private BlockMoverScript _blockMoverScript;

        private void Awake()
        {
            if (!_blockMoverScript) _blockMoverScript = GetComponent<BlockMoverScript>();
        
            if (!_blockMoverScript)
            {
                OCSFXLogger.LogError($"{nameof(BlockMoverScript)} not found on {nameof(ExitDoor)} GameObject ({name})", this, _showDebug);
            }
            
            UpdateCanInteract();
        }
        
        private void OnEnable()
        {
            ItemInventory.OnItemAdded += OnItemAdded;
            ItemInventory.OnItemRemoved += OnItemRemoved;
        }

        private void OnDisable()
        {
            ItemInventory.OnItemAdded -= OnItemAdded;
            ItemInventory.OnItemRemoved -= OnItemRemoved;
        }
        
        private void OnItemAdded(IdentifiedItem obj) => UpdateCanInteract();
        private void OnItemRemoved(IdentifiedItem obj) => UpdateCanInteract();
        

        protected override void Open()
        {
            base.Open();
        
            _blockMoverScript.Activate();
        }
        
        protected override void Close()
        {
            base.Close();
        
            _blockMoverScript.Activate();
        }
        
        private void UpdateCanInteract()
        {
            var currentInteractState = CanInteract;
            
            CanInteract = ItemInventory.Instance.ContainsAll(_requiredCollectables);
            
            if (currentInteractState != CanInteract)
            {
                DebugLog();
            }
        }

        private void DebugLog()
        {
            OCSFXLogger.Log(
                CanInteract
                    ? $"[{name}] Can now interact with {name}"
                    : $"[{name}] Can no longer interact with {name}", this, _showDebug);
        }
    }
}
