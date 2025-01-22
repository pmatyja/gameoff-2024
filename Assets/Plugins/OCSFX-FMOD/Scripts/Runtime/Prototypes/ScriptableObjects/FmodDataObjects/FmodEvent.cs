#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using FMODUnity;
using GUID = FMOD.GUID;
using UnityEngine;

namespace OCSFX.FMOD.Prototype
{
    public class FmodEvent : FmodDataObjectSO
    {
        [SerializeField] public string Path;

        [SerializeField]
        public GUID Guid;

        [SerializeField] public List<FmodBank> Banks;
        [SerializeField] public bool IsStream;
        [SerializeField] public bool Is3D;
        [SerializeField] public bool IsOneShot;
        [SerializeField] public List<FmodParameter> Parameters = new List<FmodParameter>();
        [SerializeField] public float MinDistance;
        [SerializeField] public float MaxDistance;
        [SerializeField] public int Length;

        public List<FmodParameter> LocalParameters
        {
            get { return Parameters.Where(p => p.IsGlobal == false).OrderBy(p => p.Name).ToList(); }
        }
        
        public List<FmodParameter> GlobalParameters
        {
            get { return Parameters.Where(p => p.IsGlobal == true).OrderBy(p => p.Name).ToList(); }
        }

        public void Init(string path, GUID guid, List<FmodBank> banks, bool isStream, bool is3D, bool isOneShot, List<FmodParameter> parameters, float minDistance, float maxDistance, int length)
        {
            Path = path;
            Guid = guid;
            Banks = banks;
            IsStream = isStream;
            Is3D = is3D;
            IsOneShot = isOneShot;
            Parameters = parameters;
            MinDistance = minDistance;
            MaxDistance = maxDistance;
            Length = length;
        }
        
#if UNITY_EDITOR
        public void EditorInit(EditorEventRef editorEventRef, List<FmodBank> ocsfxBanks, List<FmodParameter> ocsfxParameters)
        {
            if (!AssetIsChanged(editorEventRef, ocsfxBanks, ocsfxParameters)) return;
            
            Path = editorEventRef.Path;
            Guid = editorEventRef.Guid;
            IsStream = editorEventRef.IsStream;
            Is3D = editorEventRef.Is3D;
            IsOneShot = editorEventRef.IsOneShot;
            MinDistance = editorEventRef.MinDistance;
            MaxDistance = editorEventRef.MaxDistance;
            Length = editorEventRef.Length;

            Banks = ocsfxBanks;
            Parameters = ocsfxParameters;
            
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }       
        
        private bool AssetIsChanged(EditorEventRef editorEventRef, List<FmodBank> ocsfxBanks, List<FmodParameter> ocsfxParameters)
        {
            return Path != editorEventRef.Path ||
                   Guid != editorEventRef.Guid ||
                   IsStream != editorEventRef.IsStream ||
                   Is3D != editorEventRef.Is3D ||
                   IsOneShot != editorEventRef.IsOneShot ||
                   !Mathf.Approximately(MinDistance, editorEventRef.MinDistance) ||
                   !Mathf.Approximately(MaxDistance, editorEventRef.MaxDistance) ||
                   Length != editorEventRef.Length ||
                   !Banks.SequenceEqual(ocsfxBanks) ||
                   !Parameters.SequenceEqual(ocsfxParameters);
        }
#endif //UNITY_EDITOR
        
        public void PlayOneShot()
        {
            RuntimeManager.PlayOneShot(Path);
        }

        public EventInstance Play(GameObject sourceObject, string parameter = default, float value = 0f)
        {
            if (!sourceObject)
            {
                var dummyInstance = new EventInstance();
                dummyInstance.release();
            }

            if (!sourceObject.TryGetComponent<FMODGameObject>(out var fmodGameObject))
                fmodGameObject = sourceObject.AddComponent<FMODGameObject>();

            return
                parameter != default ? fmodGameObject.PlayEvent(Path, parameter, value) : fmodGameObject.PlayEvent(Path);
        }
        
        public void Stop(GameObject sourceObject, bool allowFadeOut = true)
        {
            if (!sourceObject) return;
            
            if (!sourceObject.TryGetComponent<FMODGameObject>(out var fmodGameObject))
                fmodGameObject = sourceObject.AddComponent<FMODGameObject>();
            
            fmodGameObject.StopEvent(Path, allowFadeOut);
        }
    }
}
