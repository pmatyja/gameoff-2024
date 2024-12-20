using System;
using System.Collections;
using System.Collections.Generic;
using OCSFX.Utility.Debug;
using UnityEngine;
using Utility.Generics;

namespace Runtime.Collectables
{
    public class ItemInventory : OCSFX.Generics.Singleton<ItemInventory>, IList<IdentifiedItem>
    {
        /**
        Note that while the ObservableList serializes, it does not display in the inspector
        If we want to see the list in the inspector, we need to create a custom inspector for this class,
        or use an Editor-only copy of the list to display in the inspector 
        */
        [SerializeField] private ObservableList<IdentifiedItem> _items = new ObservableList<IdentifiedItem>();

        [SerializeField] private bool _showDebug = true;

        public int KeyItemsCollected
        {
            get
            {
                var keyItems = _items.FindAll(i => i.Data.IsUnique);
                return keyItems.Count;
            }
        }
        
        public int OptionalItemsCollected
        {
            get
            {
                var optionalItems = _items.FindAll(i => !i.Data.IsUnique && !i.Data.IsTransient);
                return optionalItems.Count;
            }
        }

        #region Events
        public static event Action<IdentifiedItem> OnItemAdded;
        public static event Action<IdentifiedItem> OnItemRemoved;
        public static event Action<int, IdentifiedItem> OnItemInserted; 
        public static event Action<int, IdentifiedItem>  OnItemRemovedAt;
        public static event Action OnItemsCleared;
        public static event Action <int> OnKeyItemsCollectedChanged;
        public static event Action <int> OnOptionalItemsCollectedChanged;
        #endregion // Events
        
        #region IList Interface
        public IEnumerator<IdentifiedItem> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(IdentifiedItem item) => _items.Add(item);

        public void Clear() => _items.Clear();
        public bool Contains(IdentifiedItem item) => _items.Contains(item);

        public void CopyTo(IdentifiedItem[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);
        public bool Remove(IdentifiedItem item) => _items.Remove(item);

        public int Count => _items.Count;
        public bool IsReadOnly => _items.IsReadOnly;
        
        public int IndexOf(IdentifiedItem item) => _items.IndexOf(item);

        public void Insert(int index, IdentifiedItem item) => _items.Insert(index, item);

        public void RemoveAt(int index) => _items.RemoveAt(index);

        public IdentifiedItem this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }
        #endregion // IList Interface

        public void AddUnique(IdentifiedItem item)
        {
            var containsThisID = _items.Exists(i => i.ID == item.ID);
            
            if (!containsThisID)
            {
                _items.Add(item);
            }
        }
        public List<IdentifiedItem> GetItems() => _items;
        
        [RuntimeInitializeOnLoadMethod]
        private static void RuntimeInitialize()
        {
            LazyLoadInstance();
            
            _instance._items.Clear();
        }
        
        private void OnEnable()
        {
            _items.ElementAdded += OnElementAdded;
            _items.ElementInserted += OnElementInserted;
            _items.ElementRemoved += OnElementRemoved;
            _items.ElementRemovedAt += OnElementRemovedAt;
            _items.Cleared += OnElementsCleared;
        }

        private void OnDisable()
        {
            _items.ElementAdded -= OnElementAdded;
            _items.ElementInserted -= OnElementInserted;
            _items.ElementRemoved -= OnElementRemoved;
            _items.ElementRemovedAt -= OnElementRemovedAt;
            _items.Cleared -= OnElementsCleared;
        }
        
        #region Callbacks
        private void OnElementAdded(IdentifiedItem item)
        {
            OnItemAdded?.Invoke(item);
            OCSFXLogger.Log($"[{nameof(ItemInventory)}] Added {item.Data.name} to inventory", _instance, _showDebug);

            // TODO: Improve this spaghetti
            if (item.Data.IsUnique)
            {
                OnKeyItemsCollectedChanged?.Invoke(KeyItemsCollected);
                
                EventBus.Raise(this, new HudController.CubeCollectedEventsParameters
                {
                    Type = item.Data.CubeType
                });
            }
            else if (!item.Data.IsTransient)
            {
                OnOptionalItemsCollectedChanged?.Invoke(OptionalItemsCollected);
                
                EventBus.Raise(this, new HudController.ItemCollectedEventsParameters());
            }
        }

        private void OnElementInserted(int index, IdentifiedItem item)
        {
            OnItemInserted?.Invoke(index, item);
            OCSFXLogger.Log($"[{nameof(ItemInventory)}] Inserted {item.Data.name} at index {index} in inventory", _instance, _showDebug);
        }

        private void OnElementRemoved(IdentifiedItem item)
        {
            OnItemRemoved?.Invoke(item);
            OCSFXLogger.Log($"[{nameof(ItemInventory)}] Removed {item.Data.name} from inventory", _instance, _showDebug);
        }

        private void OnElementRemovedAt(int index, IdentifiedItem item)
        {
            OnItemRemovedAt?.Invoke(index, item);
            OCSFXLogger.Log($"[{nameof(ItemInventory)}] Removed {item.Data.name} at index {index} from inventory", _instance, _showDebug);
        }

        private void OnElementsCleared()
        {
            OnItemsCleared?.Invoke();
            OCSFXLogger.Log($"[{nameof(ItemInventory)}] Cleared all items from inventory", _instance, _showDebug);
        }
        #endregion // Callbacks

        public bool ContainsAll(IEnumerable<CollectableData> compareCollectables)
        {
            foreach (var collectableData in compareCollectables)
            {
                var containsThisData = _items.Exists(i => i.Data == collectableData);
                
                if (!containsThisData)
                {
                    OCSFXLogger.Log($"[{nameof(ItemInventory)}] Missing {collectableData.name} from inventory", _instance, _showDebug);
                    return false;
                }
            }
            
            OCSFXLogger.Log($"[{nameof(ItemInventory)}] Contains all required collectables", _instance, _showDebug);

            return true;
        }
        
        public bool ContainsAny(IEnumerable<CollectableData> compareCollectables)
        {
            foreach (var collectableData in compareCollectables)
            {
                var containsThisData = _items.Exists(i => i.Data == collectableData);
                
                if (containsThisData)
                {
                    OCSFXLogger.Log($"[{nameof(ItemInventory)}] Contains {collectableData.name} in inventory", _instance, _showDebug);
                    return true;
                }
            }
            
            OCSFXLogger.Log($"[{nameof(ItemInventory)}] Missing all required collectables", _instance, _showDebug);

            return false;
        }
        
        public bool ContainsAnyID(IEnumerable<string> compareIDs)
        {
            foreach (var id in compareIDs)
            {
                var containsThisID = _items.Exists(i => i.ID == id);
                
                if (containsThisID)
                {
                    OCSFXLogger.Log($"[{nameof(ItemInventory)}] Contains {id} in inventory", _instance, _showDebug);
                    return true;
                }
            }
            
            OCSFXLogger.Log($"[{nameof(ItemInventory)}] Missing all required IDs", _instance, _showDebug);

            return false;
        }
        
        public bool ContainsID(string id) => _items.Exists(i => i.ID == id);
        public bool ContainsData(CollectableData data) => _items.Exists(i => i.Data == data);
    }   
    
    [Serializable]
    public class IdentifiedItem
    {
        public string ID;
        public CollectableData Data;
            
        public IdentifiedItem(string id, CollectableData data)
        {
            ID = id;
            Data = data;
        }
    }
}
