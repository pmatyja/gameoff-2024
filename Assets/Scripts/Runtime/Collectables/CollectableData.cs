using System;
using FMODUnity;
using OCSFX.FMOD;
using Runtime.Utility;
using UnityEngine;
using OCSFX.Utility.Debug;

namespace Runtime.Collectables
{
    [CreateAssetMenu(menuName = GameOff2024Statics.MENU_ROOT + nameof(CollectableData))]
    public class CollectableData : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField] public Texture Icon { get; private set; }
        [field: SerializeField] public Material Material { get; private set; }
        
        [field: Header("Settings")]
        [field: SerializeField] public bool IsUnique { get; private set; }
        [field: SerializeField, Tooltip("Not Unique and can respawn with the scene")] public bool IsTransient { get; private set; }
        
        [field: Header("Effects")]
        [field: SerializeField] public GameObject OnCollectVfx { get; private set; }
        [field: SerializeField] public float OnCollectVfxLifetime { get; private set; } = 2f;
        [field: SerializeField] public EventReference OnCollectSfx { get; private set; }
        [field: SerializeField] public EventReference LoopSfx { get; private set; }
        
        [Header("Debug")]
        [SerializeField] private bool _showDebug;
        
        public void OnSpawn(Transform spawnTransform, string id)
        {
            if (IsUnique)
            {
                if (ItemInventory.Instance.ContainsData(this))
                {
                    OCSFXLogger.Log($"{nameof(CollectableData)} {Name} is unique and already in inventory. " +
                                    $"Destroying spawned object.", this, _showDebug);
                    
                    Destroy(spawnTransform.gameObject);
                    return;
                }
            }
            else if (!IsTransient)
            {
                if (ItemInventory.Instance.ContainsID(id))
                {
                    OCSFXLogger.Log($"{nameof(CollectableData)} {Name} is not unique or transient and is already in inventory. " +
                                    $"Destroying spawned object.", this, _showDebug);
                    
                    Destroy(spawnTransform.gameObject);
                    return;
                }
            }
            
            if (!LoopSfx.IsNull)
            {
                LoopSfx.Play(spawnTransform.gameObject);
            }
        }

        public void OnCollect(Transform collectableTransform)
        {
            HandleVfxOnCollect(collectableTransform);
            HandleSoundOnCollect(collectableTransform);
            HandleInventoryOnCollect(collectableTransform);
        }

        private void HandleVfxOnCollect(Transform collectableTransform)
        {
            if (OnCollectVfx)
            {
                var vfxInstance = Instantiate(OnCollectVfx, collectableTransform.position, collectableTransform.rotation);
                Destroy(vfxInstance, OnCollectVfxLifetime);
            }
        }

        private void HandleSoundOnCollect(Transform collectableTransform)
        {
            if (!LoopSfx.IsNull)
            {
                LoopSfx.Stop(collectableTransform.gameObject);
            }
            
            if (collectableTransform.TryGetComponent<FMODGameObject>(out var fmodGameObject))
            {
                fmodGameObject.Stop(true);
            }
            
            if (!OnCollectSfx.IsNull)
            {
                OnCollectSfx.PlayOneShot(collectableTransform.position);
            }
        }

        private void HandleInventoryOnCollect(Transform collectableTransform)
        {
            // If the collectable is transient, don't add it to the inventory
            if (IsTransient) return;
            
            var idComponent = collectableTransform.GetComponent<GameOff2024UniqueID>();
            var id = idComponent ? idComponent.ID : string.Empty;
            
            if (IsUnique)
            {
                ItemInventory.Instance.AddUnique(new IdentifiedItem(id, this));
            }
            else
            {
                ItemInventory.Instance.Add(new IdentifiedItem(id, this));
            }
        }

        private void OnValidate()
        {
            if (IsUnique)
            {
                IsTransient = false;
            }
        }
    }
}