using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using FMODUnity;
using OCSFX.EZFMOD.Attributes;
using OCSFX.EZFMOD.Debug;
using OCSFX.EZFMOD.Utility;
using UnityEngine;
using GUID = FMOD.GUID;

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
        
        public void Play(GameObject sourceObject, string parameter, float value, out EventInstance eventInstance)
        {
            if (!sourceObject || !AnyBankLoaded())
            {
                eventInstance = EZFMODRuntimeStatics.INVALID_EVENT_INSTANCE;
                return;
            }

            var fmodGameObj = sourceObject.GetOrAddComponent<EZFMODGameObject>();
            
            if (!string.IsNullOrWhiteSpace(parameter)) eventInstance = fmodGameObj.PlayEvent(StudioPath, parameter, value);
            else fmodGameObj.PlayEvent(StudioPath, out eventInstance);
        }
        
        public void Play(GameObject sourceObject, PARAMETER_ID parameterID, float value, out EventInstance eventInstance)
        {
            if (!sourceObject || !AnyBankLoaded())
            {
                eventInstance = EZFMODRuntimeStatics.INVALID_EVENT_INSTANCE;
                return;
            }

            var fmodGameObj = sourceObject.GetOrAddComponent<EZFMODGameObject>();
            
            if (parameterID.IsValid()) eventInstance = fmodGameObj.PlayEvent(StudioPath, parameterID, value);
            else fmodGameObj.PlayEvent(StudioPath, out eventInstance);
        }

        public EventInstance Play(GameObject sourceObject, string parameter, float value)
        {
            if (!sourceObject || !AnyBankLoaded())
            {
                return EZFMODRuntimeStatics.INVALID_EVENT_INSTANCE;
            }

            var fmodGameObj = sourceObject.GetOrAddComponent<EZFMODGameObject>();

            if (!string.IsNullOrWhiteSpace(parameter))
                return fmodGameObj.PlayEvent(StudioPath, parameter, value);
            
            fmodGameObj.PlayEvent(StudioPath, out var eventInstance);
            
            return eventInstance;
        }
        
        public EventInstance Play(GameObject sourceObject, PARAMETER_ID parameterID, float value)
        {
            if (!sourceObject || !AnyBankLoaded())
            {
                return EZFMODRuntimeStatics.INVALID_EVENT_INSTANCE;
            }

            var fmodGameObj = sourceObject.GetOrAddComponent<EZFMODGameObject>();

            if (parameterID.IsValid())
                return fmodGameObj.PlayEvent(StudioPath, parameterID, value);
            
            fmodGameObj.PlayEvent(StudioPath, out var eventInstance);
            
            return eventInstance;
        }
    }
}
