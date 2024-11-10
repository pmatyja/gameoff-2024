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

        private void OnItemAdded(CollectableData addedItem)
        {
            OCSFXLogger.Log($"[{name}] Item added: {addedItem.name}", this, _showDebug);
            
            CanInteract = ItemInventory.Instance.ContainsAll(_requiredCollectables);

            if (CanInteract)
            {
                OCSFXLogger.Log($"[{name}] Can now interact with {name}", this, _showDebug);
            }
        }

        private void OnItemRemoved(CollectableData addedItem)
        {
            CanInteract = ItemInventory.Instance.ContainsAll(_requiredCollectables);
            
            if (!CanInteract)
            {
                OCSFXLogger.Log($"[{name}] Can no longer interact with {name}", this, _showDebug);
            }
        }

        protected override void Open()
        {
            base.Open();
        
            _blockMoverScript.Activate();
        }
    }
}
