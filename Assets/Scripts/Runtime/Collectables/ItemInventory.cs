using System;
using System.Collections;
using System.Collections.Generic;
using OCSFX.Utility;
using OCSFX.Utility.Debug;
using UnityEngine;
using Utility.Generics;

namespace Runtime.Collectables
{
    public class ItemInventory : OCSFX.Generics.Singleton<ItemInventory>, IList<CollectableData>
    {
        /**
        Note that while the ObservableList serializes, it does not display in the inspector
        If we want to see the list in the inspector, we need to create a custom inspector for this class,
        or use an Editor-only copy of the list to display in the inspector 
        */
        [SerializeField] private ObservableList<CollectableData> _items = new ObservableList<CollectableData>();

        [SerializeField] private bool _showDebug = true;

        #region Events
        public static event Action<CollectableData> OnItemAdded;
        public static event Action<CollectableData> OnItemRemoved;
        public static event Action<int, CollectableData> OnItemInserted; 
        public static event Action<int, CollectableData>  OnItemRemovedAt;
        public static event Action OnItemsCleared;
        #endregion // Events
        
        #region IList Interface
        public IEnumerator<CollectableData> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(CollectableData item) => _items.Add(item);

        public void Clear() => _items.Clear();
        public bool Contains(CollectableData item) => _items.Contains(item);

        public void CopyTo(CollectableData[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);
        public bool Remove(CollectableData item) => _items.Remove(item);

        public int Count => _items.Count;
        public bool IsReadOnly => _items.IsReadOnly;
        
        public int IndexOf(CollectableData item) => _items.IndexOf(item);

        public void Insert(int index, CollectableData item) => _items.Insert(index, item);

        public void RemoveAt(int index) => _items.RemoveAt(index);

        public CollectableData this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }
        #endregion // IList Interface

        public void AddUnique(CollectableData item) => _items.AddUnique(item);
        public List<CollectableData> GetItems() => _items;
        
        [RuntimeInitializeOnLoadMethod]
        private static void RuntimeInitialize()
        {
            LazyLoadInstance();
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
        private void OnElementAdded(CollectableData item)
        {
            OnItemAdded?.Invoke(item);
            OCSFXLogger.Log($"[{nameof(ItemInventory)}] Added {item.name} to inventory", _instance, _showDebug);
        }

        private void OnElementInserted(int index, CollectableData item)
        {
            OnItemInserted?.Invoke(index, item);
            OCSFXLogger.Log($"[{nameof(ItemInventory)}] Inserted {item.name} at index {index} in inventory", _instance, _showDebug);
        }

        private void OnElementRemoved(CollectableData item)
        {
            OnItemRemoved?.Invoke(item);
            OCSFXLogger.Log($"[{nameof(ItemInventory)}] Removed {item.name} from inventory", _instance, _showDebug);
        }

        private void OnElementRemovedAt(int index, CollectableData item)
        {
            OnItemRemovedAt?.Invoke(index, item);
            OCSFXLogger.Log($"[{nameof(ItemInventory)}] Removed {item.name} at index {index} from inventory", _instance, _showDebug);
        }

        private void OnElementsCleared()
        {
            OnItemsCleared?.Invoke();
            OCSFXLogger.Log($"[{nameof(ItemInventory)}] Cleared all items from inventory", _instance, _showDebug);
        }
        #endregion // Callbacks
        
        
        
    }   
}
