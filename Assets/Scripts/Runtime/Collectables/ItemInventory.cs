using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Collectables
{
    public class ItemInventory : OCSFX.Generics.Singleton<ItemInventory>
    {
        [SerializeField] private List<CollectableData> _items = new List<CollectableData>();
        
        public static void Add(CollectableData item) => Instance._items.Add(item);
        public static void Remove(CollectableData item) => Instance._items.Remove(item);
        public static CollectableData GetAt(int index) => Instance._items[index];
        public static CollectableData GetFirst() => Instance._items.Count > 0 ? Instance._items[0] : null;
        public static CollectableData GetLast() => Instance._items.Count > 0 ? Instance._items[^1] : null;
        public static int Count() => Instance._items.Count;
        public static bool Contains(CollectableData item) => Instance._items.Contains(item);
        public static void Clear() => Instance._items.Clear();
        
        public static void AddUnique(CollectableData item)
        {
            if (Instance._items.Contains(item)) return;
            
            Instance._items.Add(item);
        }
    }   
}
