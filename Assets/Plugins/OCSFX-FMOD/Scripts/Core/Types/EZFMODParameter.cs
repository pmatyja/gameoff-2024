using OCSFX.EZFMOD.Attributes;
using OCSFX.EZFMOD.Utility;
using UnityEngine;
using GUID = FMOD.GUID;
using PARAMETER_ID = FMOD.Studio.PARAMETER_ID;

namespace OCSFX.EZFMOD.Types
{
    public enum ParameterType { Continuous, Discrete, Labeled }
    
    public class EZFMODParameter : EZFMODAsset
    {
        [field: SerializeField, ReadOnly] public float Min { get; private set; }
        [field: SerializeField, ReadOnly] public float Max { get; private set; }
        [field: SerializeField, ReadOnly] public float Default { get; private set; }
        [field: SerializeField, ReadOnly] public ParameterID ID { get; private set; }
        [field: SerializeField, ReadOnly] public ParameterType Type { get; private set; }
        [field: SerializeField, ReadOnly] public bool IsGlobal { get; private set; }
        [field: SerializeField, ReadOnly] public string[] Labels { get; private set; } = { };
        //
        // [field: SerializeField, ReadOnly] public bool Exists;

        internal void Init(string inName, GUID guid, string studioPath, double minValue, double maxValue, double defaultValue, PARAMETER_ID id, int parameterType, bool isGlobal, string[] labels)
        {
            Name = inName;
            GUID = guid;
            StudioPath = studioPath;
            Min = (float)minValue;
            Max = (float)maxValue;
            Default = (float)defaultValue;
            ID = id;
            Type = (ParameterType)parameterType;
            IsGlobal = isGlobal;
            Labels = labels;
            // Exists = exists;
        }
        
        public void SetGlobalValue(float value)
        {
            if (!Application.isPlaying) return;
            
            if (Type is ParameterType.Discrete or ParameterType.Labeled)
            {
                value = Mathf.Round(value);
            }
            
            EZFMODRuntimeStatics.SetGlobalParameter(ID, value);
        }

        public void SetGlobalDefaultValue()
        {
            if (!Application.isPlaying) return;
            
            SetGlobalValue(Default);
        }
        
        public void SetValue(float value, GameObject source)
        {
            if (!Application.isPlaying) return;
            
            if (Type is ParameterType.Discrete or ParameterType.Labeled)
            {
                value = Mathf.Round(value);
            }

            var fmodGameObj = source.GetOrAddComponent<EZFMODGameObject>();
            
            // TODO: See if there's a better solution than checking for parameter descriptions from active events
            // Because setting parameter by ID relies on getting parameter information from event descriptions for local parameters,
            // it won't work to store a parameter on the EZFMODGameObject.
            // Using the name gets around this, but is less efficient.
            // fmodGameObj.SetParameter(ID, value);
            
            fmodGameObj.SetParameter(Name, value);
        }

        public void SetDefaultValue(GameObject source)
        {
            if (!Application.isPlaying) return;

            SetValue(Default, source);
        }
        
        [System.Serializable]
        public struct ParameterID
        {
            public uint data1;
            public uint data2;
            
            public ParameterID(uint data1, uint data2)
            {
                this.data1 = data1;
                this.data2 = data2;
            }
            
            public ParameterID(PARAMETER_ID id)
            {
                data1 = id.data1;
                data2 = id.data2;
            }
            
            public static implicit operator ParameterID(PARAMETER_ID source)
            {
                return new ParameterID {
                    data1 = source.data1,
                    data2 = source.data2,
                };
            }
            
            public static implicit operator PARAMETER_ID(ParameterID source)
            {
                return new PARAMETER_ID {
                    data1 = source.data1,
                    data2 = source.data2,
                };
            }

            public bool Equals(PARAMETER_ID other)
            {
                return data1 == other.data1 && data2 == other.data2;
            }
            
            public bool Equals(ParameterID other)
            {
                return data1 == other.data1 && data2 == other.data2;
            }
            
            // public override int GetHashCode()
            // {
            //     return HashCode.Combine(data1, data2);
            // }
        }
    }
}
