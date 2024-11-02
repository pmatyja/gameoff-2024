using System;
using System.Collections.Generic;
using UnityEngine;

namespace OCSFX.FMOD.Prototype
{
    /// <summary>
    /// This script is a work in progress. Do not use.
    /// </summary>
    [Serializable]
    public class SerializableDictionary<K, V>
    {
        // This does not work, because we cannot automatically remove duplicate keys.
        // It could work as a ScriptableObject so it can run its own OnValidate...
        // TODO: Try to make it work with a custom Inspector/Editor/Drawer class.
        
        [SerializeField] private List<KeyValuePair<K, V>> _entries = new List<KeyValuePair<K, V>>();
        private Dictionary<K, V> _internalDictionary = new Dictionary<K, V>();

        public V Get(K key) => _internalDictionary[key];
        public bool TryGet(K key, out V value) => _internalDictionary.TryGetValue(key, out value);
        
        public void Add(K key, V value) => InternalAdd(key, value);
        public void TryAdd(K key, V value) => TryInternalAdd(key, value);

        private void InternalAdd(K key, V value)
        {
            var pair = new KeyValuePair<K, V>(key, value);
            if (_entries.Contains(pair)) return;
            
            _entries.Add(pair);
            _internalDictionary.Add(key, value);
        }
        
        private bool TryInternalAdd(K key, V value)
        {
            var pair = new KeyValuePair<K, V>(key, value);
            if (_entries.Contains(pair)) return false;
            
            _entries.Add(pair);
            _internalDictionary.Add(key, value);
            return true;
        }
        
        public void Refresh()
        {
            // Remove duplicates
            var trimmedList = new List<KeyValuePair<K, V>>();
            var keys = new HashSet<K>();
            foreach (var entry in _entries)
            {
                if (keys.Contains(entry.Key))
                {
                    if (!string.IsNullOrEmpty(entry.Key.ToString()))
                    {
                        trimmedList.Add(default);
                    }
                    continue;
                }
                keys.Add(entry.Key);
                trimmedList.Add(entry);
            }
            
            _entries.Clear();
            _entries.AddRange(trimmedList);

            // Refresh the internal Dictionary
            _internalDictionary.Clear();
            foreach (var entry in _entries)
            {
                if (entry.Key == null) continue;
                _internalDictionary.TryAdd(entry.Key, entry.Value);
            }
        }
    }
}
