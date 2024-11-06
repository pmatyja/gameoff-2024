using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameOff2024.SceneLoading
{
    [CreateAssetMenu(fileName = nameof(TooltipCollectionSO), menuName = "Collection/" + nameof(TooltipCollectionSO), order = 0)]
    public class TooltipCollectionSO : ScriptableObject
    {
        [SerializeField] private List<Tooltip> _collection = new List<Tooltip>();

        private int _lastRetrievedIndex;

        public Tooltip GetTooltip(string key)
        {
            var result = _collection.Find((entry) => entry.Key == key);
            _lastRetrievedIndex = _collection.IndexOf(result);
        
            return result;
        }

        public Tooltip GetRandom()
        {
            var randomIndex = Random.Range(0, _collection.Count);
            _lastRetrievedIndex = randomIndex;
        
            return _collection[randomIndex];
        }
    
        public Tooltip GetNext()
        {
            var result = _collection[_lastRetrievedIndex++ % _collection.Count];
            return result;
        }
    
        public Tooltip GetPrevious()
        {
            var result = _collection[_lastRetrievedIndex-- % _collection.Count];
            return result;
        }
    }

    [Serializable]
    public struct Tooltip
    {
        [SerializeField] private string _key;
        [SerializeField] private string _header;
        [SerializeField, TextArea] private string _text;

        public string Key => _key;
        public string Header => _header;
        public string Text => _text;
    }
}