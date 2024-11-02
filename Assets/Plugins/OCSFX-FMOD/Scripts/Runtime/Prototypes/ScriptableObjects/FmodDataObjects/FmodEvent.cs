using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using FMODUnity;
using OCSFX.FMOD.Components;
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
        [SerializeField] public List<FmodParameter> Parameters;
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
