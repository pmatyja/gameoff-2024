using System.Collections.Generic;
using System.Linq;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using OCSFX.EZFMOD.Attributes;
using OCSFX.EZFMOD.Utility;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace OCSFX.EZFMOD.Types
{
    public abstract class EZFMODAsset : ScriptableObject
    {
        [field: SerializeField, ReadOnly] public string Name { get; protected set; }

        [field: SerializeField, ReadOnly] public string StudioPath { get; protected set; }

        [field: SerializeField, ReadOnly] public GUID GUID { get; protected set; }
    }

    public abstract class EZFMODEventBase : EZFMODAsset, IEZFMODInstantiable
    {
        public List<EventInstance> EventInstances => EZFMODRuntimeStatics.GetEventInstancesFromStudioPath(StudioPath).ToList();

        public EventReference GetEventReference() => RuntimeManager.PathToEventReference(StudioPath);

        public void PlayOneShot() => RuntimeManager.PlayOneShot(GUID);

        public void Play2D() => GetEventReference().Play2D();
        
        public void Play2D(out EventInstance eventInstance) => eventInstance = GetEventReference().Play2D();
        
        public void Play(GameObject sourceObject) => Play(sourceObject, out _);

        public void Play(GameObject sourceObject, out EventInstance eventInstance)
        {
            eventInstance = EZFMODRuntimeStatics.INVALID_EVENT_INSTANCE;
            if (!sourceObject) return;

            var fmodGameObj = sourceObject.GetOrAddComponent<EZFMODGameObject>();
            
            fmodGameObj.PlayEvent(StudioPath, out eventInstance);
        }

        public void Stop(GameObject sourceObject, bool allowFadeOut)
        {
            if (!sourceObject) return;
            
            if (!sourceObject.TryGetComponent<EZFMODGameObject>(out var fmodGameObject))
                fmodGameObject = sourceObject.AddComponent<EZFMODGameObject>();
            
            fmodGameObject.StopEvent(GUID, allowFadeOut);
        }
        
        public void Stop(GameObject sourceObject)
            => Stop(sourceObject, true);
        
        public void StopAll(bool allowFadeOut)
        {
            foreach (var eventInstance in EventInstances)
            {
                eventInstance.stop(allowFadeOut ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
            }
        }

        public void StopAll()
        {
            foreach (var eventInstance in EventInstances)
            {
                eventInstance.stop(STOP_MODE.IMMEDIATE);
            }
        }
    }
}