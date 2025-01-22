#if UNITY_EDITOR
using System;
using FMODUnity;
using UnityEditor;
#endif //UNITY_EDITOR

using System.Linq;
using OCSFX.Utility;
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
        
#if UNITY_EDITOR
        public void EditorInit(EditorParamRef editorParamRef)
        {
            if (!AssetIsChanged(editorParamRef)) return;
            
            Name = editorParamRef.Name;
            StudioPath = editorParamRef.StudioPath;
            Min = editorParamRef.Min;
            Max = editorParamRef.Max;
            Default = editorParamRef.Default;
            ID = new ParameterID(editorParamRef.ID);
            Type = (ParameterType)editorParamRef.Type;
            IsGlobal = editorParamRef.IsGlobal;
            Labels = editorParamRef.Labels;
            
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }
        
        private bool AssetIsChanged(EditorParamRef editorParamRef)
        {
            bool isChanged = false;
            
            isChanged |= Name != editorParamRef.Name;
            isChanged |= StudioPath != editorParamRef.StudioPath;
            isChanged |= !Mathf.Approximately(Min, editorParamRef.Min);
            isChanged |= !Mathf.Approximately(Max, editorParamRef.Max);
            isChanged |= !Mathf.Approximately(Default, editorParamRef.Default);
            isChanged |= ID.data1 != editorParamRef.ID.data1 || ID.data2 != editorParamRef.ID.data2;
            isChanged |= Type != (ParameterType)editorParamRef.Type;
            isChanged |= IsGlobal != editorParamRef.IsGlobal;
            isChanged |= !Labels.HasSameContentAs(editorParamRef.Labels);
            isChanged |= Exists != editorParamRef.Exists;

            return isChanged;
        }
#endif //UNITY_EDITOR
         
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
        }
    }
}
