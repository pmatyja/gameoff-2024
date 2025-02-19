using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using FMODUnity;
using OCSFX.EZFMOD.Attributes;
using OCSFX.EZFMOD.Debug;
using OCSFX.EZFMOD.Utility;
using UnityEngine;
using GUID = FMOD.GUID;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace OCSFX.EZFMOD.Types
{
    public class EZFMODEvent : EZFMODEventBase
    {
        [field: SerializeField, HideInInspector] internal GUID[] BankGUIDs { get; private set; }
        [field: SerializeField, ReadOnly] public EZFMODBank[] Banks { get; private set; }
        [field: SerializeField, HideInInspector] internal GUID[] ParameterGUIDs { get; private set; }
        [field: SerializeField, ReadOnly] public EZFMODParameter[] Parameters { get; private set; }
        
        [field:Space]
        [field: SerializeField, ReadOnly] public bool Is3D { get; private set; }
        [field: SerializeField, ReadOnly] public bool IsOneShot { get; private set; }
        
        [field: Space]
        [field: SerializeField, ReadOnly] public float MinDistance { get; private set; }
        [field: SerializeField, ReadOnly] public float MaxDistance { get; private set; }
        [field: SerializeField, ReadOnly] public float Length { get; private set; }
        
        internal void Init(string inName, string path, GUID guid, GUID[] banks, bool is3D, bool isOneShot, GUID[] parameters, double minDistance, double maxDistance, double length)
        {
            Name = inName;
            StudioPath = path;
            GUID = guid;
            BankGUIDs = banks;
            Is3D = is3D;
            IsOneShot = isOneShot;
            ParameterGUIDs = parameters;
            MinDistance = (float)minDistance;
            MaxDistance = (float)maxDistance;
            Length = (float)length;
        }
        
        internal void SetBankObjects(EZFMODBank[] bankObjects) => Banks = bankObjects;
        internal void SetParameterObjects(EZFMODParameter[] parameterObjects) => Parameters = parameterObjects;
        
        // TODO: Deserialize more data into this object
        /*, List<FmodBank> banks, bool isStream, bool is3D, bool isOneShot, List<FmodParameter> parameters, float minDistance, float maxDistance, int length*/
        
        public bool AnyBankLoaded()
        {
            if (Banks == null) return false;
            if (!RuntimeManager.IsInitialized) return false;

            foreach (var bank in Banks)
            {
                // If any referenced bank is loaded, we should be good to go.
                if (RuntimeManager.HasBankLoaded(bank.Name)) break;
                
                OCSFXLogger.LogWarning($"Event ({Name}) cannot play because bank ({bank.Name}) is not loaded.", this);
                return false;
            }

            return true;
        }
    }
    
    public abstract class EZFMODEventBase : EZFMODAsset, IEZFMODInstantiable
    {
        public List<EventInstance> EventInstances => RuntimeManager.IsInitialized
            ? EZFMODRuntimeStatics.GetEventInstancesFromGUID(GUID).ToList()
            : new List<EventInstance>();

        public EventReference GetEventReference() => RuntimeManager.IsInitialized
            ? RuntimeManager.PathToEventReference(StudioPath)
            : EZFMODRuntimeStatics.INVALID_EVENT_REFERENCE;

        public void PlayOneShot() => 
            EZFMODRuntimeStatics.RunOnStartupBanksLoaded(()=>RuntimeManager.PlayOneShot(GUID));
        
        public void PlayOneShot(Vector3 position) => 
            EZFMODRuntimeStatics.RunOnStartupBanksLoaded(()=>RuntimeManager.PlayOneShot(GUID, position));

        public void Play2D() => 
            EZFMODRuntimeStatics.RunOnStartupBanksLoaded(()=>GetEventReference().Play2D());
        
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
            EZFMODRuntimeStatics.RunOnStartupBanksLoaded(() => StopAllEventInstances(allowFadeOut));
        }

        public void StopAll()
        {
            EZFMODRuntimeStatics.RunOnStartupBanksLoaded(StopAllEventInstances);
        }
        
        private void StopAllEventInstances(bool allowFadeOut)
        {
            foreach (var eventInstance in EventInstances)
            {
                eventInstance.stop(allowFadeOut ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
            }
        }
        
        private void StopAllEventInstances()
        {
            foreach (var eventInstance in EventInstances)
            {
                eventInstance.stop(STOP_MODE.IMMEDIATE);
            }
        }
    }
}
