using OCSFX.EZFMOD.Attributes;
using OCSFX.EZFMOD.Debug;
using OCSFX.EZFMOD.Utility;
using UnityEngine;

namespace OCSFX.EZFMOD.Types
{
    public class EZFMODParameterValue : ScriptableObject
    {
        [SerializeField, ReadOnly] private EZFMODParameter _parameter;
        [SerializeField, ReadOnly] private float _value;
        
        public EZFMODParameter Parameter => _parameter;
        public float Value => _value;
        
        internal void Init(EZFMODParameter parameter, float value)
        {
            _parameter = parameter;
            _value = value;

            var generatedName = _parameter.Name;
            
            switch (parameter.Type)
            {
                case ParameterType.Continuous:
                    generatedName += $"_Continuous";
                    break;
                case ParameterType.Discrete:
                    generatedName += $"_{Mathf.RoundToInt(value)}";
                    break;
                case ParameterType.Labeled:
                    generatedName += $"_{_parameter.Labels[Mathf.RoundToInt(value)]}";
                    break;    
            }
            
            name = generatedName;
        }

        public void SetGlobal()
        {
            if (_parameter.IsGlobal)
            {
                _parameter.SetGlobalValue(_value);
            }
            else
            {
                OCSFXLogger.LogWarning(
                    $"Parameter {_parameter.Name} is not global. " +
                    "Please provide a GameObject to set a local parameter value.", this);
            }
        }
        
        public void SetValue(float value, bool applyGlobal)
        {
            _value = value;
            
            if (applyGlobal)
            {
                SetGlobal();
            }
        }
        
        public void SetValue(float value, GameObject applyToTarget)
        {
            _value = value;
            Set(applyToTarget);
        }

        public void SetValue(float value)
        {
            _value = value;
        }
        
        public void Set(GameObject source)
        {
            if (!source) return;
            
            if (_parameter.IsGlobal)
            {
                OCSFXLogger.LogWarning(
                    $"Parameter {_parameter.Name} is global. " +
                    "Providing a GameObject is unnecessary.", this);
                
                _parameter.SetGlobalValue(_value);
            }
            else
            {
                source.GetOrAddComponent<EZFMODGameObject>().SetParameter(_parameter.ID, _value);
            }
        }
        
        public void SetGlobalDefault()
        {
            if (_parameter.IsGlobal)
            {
                _parameter.SetGlobalDefaultValue();
            }
            else
            {
                OCSFXLogger.LogWarning(
                    $"Parameter {_parameter.Name} is not global. " +
                    "Please provide a GameObject to set a local parameter value.", this);
            }
        }
        
        public void SetDefault(GameObject source)
        {
            if (!source) return;
            
            if (_parameter.IsGlobal)
            {
                OCSFXLogger.LogWarning(
                    $"Parameter {_parameter.Name} is global. " +
                    "Providing a GameObject is unnecessary.", this);
                
                _parameter.SetGlobalDefaultValue();
            }
            else
            {
                source.GetOrAddComponent<EZFMODGameObject>().SetParameter(_parameter.ID, (float)_parameter.Default);
            }
        }
    }
}