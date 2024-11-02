using UnityEngine;
using PARAMETER_ID = FMOD.Studio.PARAMETER_ID;

namespace OCSFX.FMOD.Prototype
{
    public enum ParameterType { Continuous, Discrete, Labeled }

    public class FmodParameter : FmodDataObjectSO
    {
        [SerializeField]
        public string Name;
        [SerializeField]
        public string StudioPath;
        [SerializeField]
        public float Min;
        [SerializeField]
        public float Max;
        [SerializeField]
        public float Default;
        [SerializeField]
        public ParameterID ID;
        [SerializeField]
        public ParameterType Type;
        [SerializeField]
        public bool IsGlobal;
        [SerializeField]
        public string[] Labels = { };
 
        public bool Exists;

        public void Init(string inName, string studioPath, float minValue, float maxValue, float defaultValue, PARAMETER_ID id, ParameterType type, bool isGlobal, string[] labels, bool exists)
        {
            Name = inName;
            StudioPath = studioPath;
            Min = minValue;
            Max = maxValue;
            Default = defaultValue;
            ID = id;
            Type = type;
            IsGlobal = isGlobal;
            Labels = labels;
            Exists = exists;
        }
         
        [System.Serializable]
        public struct ParameterID
        {
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

            public uint data1;
            public uint data2;
        }
    }
}
