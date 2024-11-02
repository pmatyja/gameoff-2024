using UnityEngine;

namespace OCSFX.Generics
{
    [System.Serializable]
    public class KeyValuePair<K, V>
    {
        [SerializeField] K _key;
        [SerializeField] V _value;

        public K Key
        {
            get => _key;
            set => _key = value;
        }

        public V Value
        {
            get => _value;
            set => _value = value;
        }

        public KeyValuePair()
        {
            _key = default;
            _value = default;
        }

        public KeyValuePair(K key, V value)
        {
            _key = key;
            _value = value;
        }
    }
}
