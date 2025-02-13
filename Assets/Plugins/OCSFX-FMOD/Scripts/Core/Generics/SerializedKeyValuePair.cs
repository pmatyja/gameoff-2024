using UnityEngine;

namespace OCSFX.EZFMOD.Utility.Generics
{
    [System.Serializable]
    public class SerializedKeyValuePair<K, V>
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

        public SerializedKeyValuePair()
        {
            _key = default;
            _value = default;
        }

        public SerializedKeyValuePair(K key, V value)
        {
            _key = key;
            _value = value;
        }
    }
}
