using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


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
public struct Tooltip : IEquatable<Tooltip>
{
    [SerializeField] private string _key;
    [SerializeField] private string _header;
    [SerializeField, TextArea] private string _text;

    public string Key => _key;
    public string Header => _header;
    public string Text => _text;

    public bool Equals(Tooltip other)
    {
        return _key == other._key && _header == other._header && _text == other._text;
    }

    public override bool Equals(object comparison)
    {
        return comparison is Tooltip other && Equals(other);
    }

    public override int GetHashCode()
    {
        return System.HashCode.Combine(_key, _header, _text);
    }
    
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(_key) && !string.IsNullOrEmpty(_header) && !string.IsNullOrEmpty(_text);
    }
}
