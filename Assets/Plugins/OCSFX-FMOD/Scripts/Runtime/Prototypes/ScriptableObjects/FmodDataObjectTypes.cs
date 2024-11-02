using System;
using System.Collections.Generic;
using FMOD.Studio;
using OCSFX.Attributes;
using UnityEngine;

namespace OCSFX.FMOD.Prototype
{
    [Serializable]
    internal struct FmodParameterObjectData
    {
        [SerializeField, ReadOnly] private PARAMETER_ID _id;
        [SerializeField, ReadOnly] private bool _isGlobal;
        [SerializeField, ReadOnly] private bool _exists;
        [SerializeField, ReadOnly] private ParameterType _type;
        [Space]
        [SerializeField, ReadOnly] private float _minValue;
        [SerializeField, ReadOnly] private float _maxValue;
        [SerializeField, ReadOnly] private float _defaultValue;
        [SerializeField, ReadOnly] private string[] _labels;
        
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
        
        private bool SetAndCheckChange<T>(ref T field, T newValue)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue)) return false;
            field = newValue;
            return true;
        }
            
        public bool IsNull()
        {
            return Equals(default(FmodParameterObjectData));
        }
    }
}