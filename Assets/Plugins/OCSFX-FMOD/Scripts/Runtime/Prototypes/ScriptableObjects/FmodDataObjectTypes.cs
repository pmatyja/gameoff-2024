using System;
using System.Collections.Generic;
using FMOD.Studio;
using OCSFX.Attributes;
using OCSFX.Utility;
using UnityEngine;

namespace OCSFX.FMOD.Prototype
{
    [Serializable]
    internal struct FmodParameterObjectData : IEquatable<FmodParameterObjectData>
    {
        [SerializeField] private FmodParameter.ParameterID _id;
        [SerializeField] private bool _isGlobal;
        [SerializeField] private bool _exists;
        [SerializeField] private ParameterType _type;
        [Space]        
        [SerializeField] private float _minValue;
        [SerializeField] private float _maxValue;
        [SerializeField] private float _defaultValue;
        [SerializeField] private string[] _labels;
        
        public PARAMETER_ID ID => _id;
        public bool IsGlobal => _isGlobal;
        public string[] Labels => _labels;
        public float MinValue => _minValue;
        public float MaxValue => _maxValue;
        public float DefaultValue => _defaultValue;
        public ParameterType Type => _type;
        public bool Exists => _exists;

        public FmodParameterObjectData(
            PARAMETER_ID id, 
            bool isGlobal, 
            string[] labels, 
            float minValue, 
            float maxValue, 
            float defaultValue, 
            ParameterType type, 
            bool exists
            )
        {
            _id = id;
            _isGlobal = isGlobal;
            _labels = labels;
            _minValue = minValue;
            _maxValue = maxValue;
            _defaultValue = defaultValue;
            _type = type;
            _exists = exists;
        }

        public bool SetData(
            PARAMETER_ID id, 
            bool isGlobal, 
            string[] labels, 
            float minValue, 
            float maxValue, 
            float defaultValue, 
            ParameterType type, 
            bool exists
            )
        {
            var didChange = false;

            didChange |= SetAndCheckChange(ref _id, id);
            didChange |= SetAndCheckChange(ref _isGlobal, isGlobal);
            didChange |= SetAndCheckChange(ref _labels, labels);
            didChange |= SetAndCheckChange(ref _minValue, minValue);
            didChange |= SetAndCheckChange(ref _maxValue, maxValue);
            didChange |= SetAndCheckChange(ref _defaultValue, defaultValue);
            didChange |= SetAndCheckChange(ref _type, type);
            didChange |= SetAndCheckChange(ref _exists, exists);

            return didChange;
        }
        
        private static bool SetAndCheckChange<T>(ref T field, T newValue)
        {
            if (field is PARAMETER_ID id)
            {
                if (Helpers.IDsAreEqual(id, (PARAMETER_ID) (object) newValue)) return false;
            }
            else if (field is string[] labels)
            {
                if (Helpers.LabelsAreEqual(labels, (string[]) (object) newValue)) return false;
            }
            else if (field.Equals(newValue)) return false;
            
            field = newValue;
            return true;
        }
            
        public bool IsNull()
        {
            return Equals(default);
        }

        public bool Equals(FmodParameterObjectData other)
        {
            return Helpers.IDsAreEqual(_id, other._id)
                   && _isGlobal == other._isGlobal
                   && _exists == other._exists
                   && _type == other._type
                   && _minValue.Equals(other._minValue)
                   && _maxValue.Equals(other._maxValue)
                   && _defaultValue.Equals(other._defaultValue)
                   && Helpers.LabelsAreEqual(_labels, other._labels);
            
        }
    }

    public static class Helpers
    {
        public static bool IDsAreEqual(PARAMETER_ID a, PARAMETER_ID b)
        {
            return a.data1 == b.data1 && a.data2 == b.data2;
        }
        
        public static bool LabelsAreEqual(ReadOnlySpan<string> a, ReadOnlySpan<string> b)
        {
            if (a.Length != b.Length) return false;

            for (var i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) return false;
            }

            return true;
        }
    }
}