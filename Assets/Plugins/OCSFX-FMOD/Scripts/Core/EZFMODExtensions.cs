using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using OCSFX.EZFMOD.Debug;
using OCSFX.EZFMOD.Types;
using UnityEngine;
using GUID = FMOD.GUID;
using Object = UnityEngine.Object;
using STOP_MODE = FMOD.Studio.STOP_MODE;
using PARAMETER_ID = FMOD.Studio.PARAMETER_ID;

namespace OCSFX.EZFMOD
{
    public static class EZFMODExtensions
    {
        private static bool IsRuntimeManagerInitialized(string attemptedAction)
        {
            var isInitialized = RuntimeManager.IsInitialized;
            if (!isInitialized)
            {
                OCSFXLogger.LogError($"[{nameof(EZFMODExtensions)}] Tried to {attemptedAction} before FMOD {nameof(RuntimeManager)} was initialized.");
            }
            
            return isInitialized;
        }
        
        public static void PlayOneShot(this EventReference eventRef, Vector3 position = default)
        {
            if (!IsRuntimeManagerInitialized($"{nameof(PlayOneShot)}")) return;
            
            RuntimeManager.PlayOneShot(eventRef, position);
        }

        public static void PlayOneShotAttached(this EventReference eventRef, GameObject soundSource)
        {
            if (!IsRuntimeManagerInitialized($"{nameof(PlayOneShotAttached)}")) return;
            
            RuntimeManager.PlayOneShotAttached(eventRef, soundSource);
        }

        public static EventInstance Play(this EventReference eventRef, GameObject sourceObject)
        {
            if (!IsRuntimeManagerInitialized($"{nameof(Play)}")) return EZFMODRuntimeStatics.INVALID_EVENT_INSTANCE;
            
            if (!sourceObject) return EZFMODRuntimeStatics.INVALID_EVENT_INSTANCE;

            if (!sourceObject.TryGetComponent<EZFMODGameObject>(out var fmodGameObject)) 
                fmodGameObject = sourceObject.AddComponent<EZFMODGameObject>();
            
            fmodGameObject.PlayEvent(eventRef, out var eventInstance);

            return eventInstance;
        }

        public static EventInstance Play2D(this EventReference eventRef, 
            string parameter = null, float value = 0)
        {
            if (!IsRuntimeManagerInitialized($"{nameof(Play2D)}")) return EZFMODRuntimeStatics.INVALID_EVENT_INSTANCE;
            
            var newInstance = RuntimeManager.CreateInstance(eventRef);

            if (parameter != null) newInstance.setParameterByName(parameter, value);

            newInstance.getDescription(out var eventDesc);
            eventDesc.is3D(out var is3D);
            if (is3D)
            {
                var listener3dAttributes = 
#if UNITY_6000_0_OR_NEWER
                    Object.FindFirstObjectByType<StudioListener>().transform.To3DAttributes();
#else
                    Object.FindObjectOfType<StudioListener>().transform.To3DAttributes();
#endif
                
                newInstance.set3DAttributes(listener3dAttributes);
            }

            newInstance.start();
            newInstance.release();

            return newInstance;
        }
        
        public static void Stop(this EventReference eventRef, GameObject sourceObject, bool allowFadeout = true)
        {
            if (!IsRuntimeManagerInitialized($"{nameof(Stop)}")) return;
            
            if (!sourceObject) return;
            
            if (!sourceObject.TryGetComponent<EZFMODGameObject>(out var fmodGameObject))
                fmodGameObject = sourceObject.AddComponent<EZFMODGameObject>();
            
            fmodGameObject.StopEvent(eventRef, allowFadeout);
        }
        
        public static void Stop2D(this EventReference eventRef, bool allowFadeout = true)
        {
            if (!IsRuntimeManagerInitialized($"{nameof(Stop2D)}")) return;
            
            var eventDesc = RuntimeManager.GetEventDescription(eventRef);
            
            eventDesc.getInstanceList(out var instanceList);

            if (instanceList.Length < 1) return;

            var stopMode = allowFadeout ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE;

            instanceList[0].stop(stopMode);
            instanceList[0].release();
        }
        
        public static void StopGlobal(this EventReference eventRef, bool allowFadeout = true)
        {
            if (!IsRuntimeManagerInitialized($"{nameof(StopGlobal)}")) return;

            if (eventRef.IsNull) return;
            
            var eventDesc = RuntimeManager.GetEventDescription(eventRef);
            
            eventDesc.getInstanceList(out var instanceList);

            if (instanceList.Length < 1) return;

            var stopMode = allowFadeout ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE;
            foreach (var instance in instanceList)
            {
                instance.stop(stopMode);
                instance.release();
            }
        }
        
        public static string GetEventStudioPath(this EventReference eventReference)
        {
            if (!IsRuntimeManagerInitialized($"{nameof(GetEventStudioPath)}")) return string.Empty;
            
            return eventReference.ToString();
        }
        
        public static string GetEventName(this EventReference eventReference)
        {
            if (!IsRuntimeManagerInitialized($"{nameof(GetEventName)}")) return string.Empty;
            
            var eventPath = eventReference.ToString();

            var segments = eventPath.Split("/");
            var eventName = segments[^1];

            return eventName;
        }
        
        public static double GetDuration(this EventReference eventReference)
        {
            if (!IsRuntimeManagerInitialized($"{nameof(GetDuration)}")) return 0;

            var eventDesc = RuntimeManager.GetEventDescription(eventReference);
            
            eventDesc.getLength(out var durationInMS);
            var durationInSeconds = durationInMS / 1000.0;
            
            return durationInSeconds;
        }
        
        public static double GetDuration(this EventInstance instance)
        {
            if (!IsRuntimeManagerInitialized($"{nameof(GetDuration)}")) return 0;
            
            instance.getDescription(out var eventDesc);
            eventDesc.getLength(out var durationInMS);
            var durationInSeconds = durationInMS / 1000.0;
            
            return durationInSeconds;
        }
        
        public static void Stop(this EventInstance instance, bool allowFadeout = true)
        {
            if (!IsRuntimeManagerInitialized($"{nameof(Stop)}")) return;
            
            if (!instance.isValid())
            {
                OCSFXLogger.LogWarning(nameof(EZFMODExtensions) + " tried to stop an invalid event instance.");
                return;
            }

            var stopMode = allowFadeout ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE;
            instance.stop(stopMode);
            instance.release();
        }

        public static string GetEventName(this EventInstance eventInstance)
        {
            if (!IsRuntimeManagerInitialized($"{nameof(GetEventName)}")) return string.Empty;
            
            eventInstance.getDescription(out var eventDescription);
            eventDescription.getPath(out var eventPath);

            var segments = eventPath.Split("/");
            var eventName = segments[^1];

            return eventName;
        }
        
        public static GUID GetEventGUID(this EventInstance eventInstance)
        {
            if (!IsRuntimeManagerInitialized($"{nameof(GetEventGUID)}")) return default;
            
            eventInstance.getDescription(out var eventDescription);
            eventDescription.getID(out var eventGUID);
            
            return eventGUID;
        }
        
        public static float GetEventVolume(this EventInstance eventInstance)
        {
            if (!IsRuntimeManagerInitialized($"{nameof(GetEventVolume)}")) return 0;
            
            eventInstance.getVolume(out var volume);
            return volume;
        }

        public static void SetEventVolume(this EventInstance eventInstance, float multiplier = 1)
        {
            if (!IsRuntimeManagerInitialized($"{nameof(SetEventVolume)}")) return;
            
            if (!eventInstance.isValid()) return;
            
            multiplier = Mathf.Clamp(multiplier, 0, 2);
            
            eventInstance.getVolume(out var instanceVolume);
            eventInstance.setVolume(multiplier * instanceVolume);
        }
        
        public static void SetParameter(this GameObject sourceObject, string parameterName, float value)
        {
            if (!IsRuntimeManagerInitialized($"{nameof(SetParameter)}")) return;
            
            if (!sourceObject) return;

            if (!sourceObject.TryGetComponent<EZFMODGameObject>(out var fmodGameObject))
            {
                fmodGameObject = sourceObject.AddComponent<EZFMODGameObject>();
            }
            
            fmodGameObject.SetParameter(parameterName,value);
        }
        
        public static bool TryGetParameter(this List<FMODParameter> fmodParamStructs, string structName, out string parameter)
        {
            if (!IsRuntimeManagerInitialized($"{nameof(TryGetParameter)}")) 
            {
                parameter = string.Empty;
                return false;
            }
            
            parameter = fmodParamStructs.GetParameter(structName);
            return !string.IsNullOrWhiteSpace(parameter);
        }

        public static string GetParameter(this List<FMODParameter> fmodParamStructs, string structName)
        {
            if (!IsRuntimeManagerInitialized($"{nameof(GetParameter)}")) return string.Empty;
            
            return fmodParamStructs.Find(fmodParamStruct => fmodParamStruct.Parameter == structName).Parameter;
        }
        
        public static bool TryGetParameter(this List<FMODGlobalParameter> fmodParamStructs, string structName, out string parameter)
        {
            if (!IsRuntimeManagerInitialized($"{nameof(TryGetParameter)}")) 
            {
                parameter = string.Empty;
                return false;
            }
            
            parameter = fmodParamStructs.GetParameter(structName);
            return !string.IsNullOrWhiteSpace(parameter);
        }

        public static string GetParameter(this List<FMODGlobalParameter> fmodParamStructs, string structName)
        {
            if (!IsRuntimeManagerInitialized($"{nameof(GetParameter)}")) return string.Empty;
            
            return fmodParamStructs.Find(fmodParamStruct => fmodParamStruct.Parameter == structName).Parameter;
        }
        
        public static bool TryGetEventReference(this List<FMODEvent> fmodEventStructs, string structName, out EventReference eventReference)
        {
            if (!IsRuntimeManagerInitialized($"{nameof(TryGetEventReference)}")) 
            {
                eventReference = default;
                return false;
            }
            
            eventReference = fmodEventStructs.GetEventReference(structName);
            return !eventReference.IsNull;
        }

        public static EventReference GetEventReference(this List<FMODEvent> fmodEventStructs, string structName)
        {
            if (!IsRuntimeManagerInitialized($"{nameof(GetEventReference)}")) return default;
            
            return fmodEventStructs.Find(fmodEventStruct => fmodEventStruct.Name == structName).EventRef;
        }
        
        public static bool TryGetBank(this List<FMODBank> fmodBankStructs, string structName, out string bank)
        {
            if (!IsRuntimeManagerInitialized($"{nameof(TryGetBank)}")) 
            {
                bank = string.Empty;
                return false;
            }
            
            bank = fmodBankStructs.GetBank(structName);
            return !string.IsNullOrWhiteSpace(bank);
        }
        
        public static string GetBank(this List<FMODBank> fmodBankStructs, string structName)
        {
            if (!IsRuntimeManagerInitialized($"{nameof(GetBank)}")) return string.Empty;
            
            return fmodBankStructs.Find(fmodBankStruct => fmodBankStruct.Bank == structName).Bank;
        }
        
        public static bool Equals(this PARAMETER_ID source, PARAMETER_ID other)
        {
            return source.data1 == other.data1 && source.data2 == other.data2;
        }
        
        public static bool Equals(this EZFMODParameter.ParameterID source, PARAMETER_ID other)
        {
            return source.data1 == other.data1 && source.data2 == other.data2;
        }
        
        public static bool IsValid(this PARAMETER_ID parameterID)
        {
            return parameterID.data1 != 0 && parameterID.data2 != 0;
        }
        
        public static bool IsValid(this PARAMETER_DESCRIPTION parameterDescription)
        {
            return parameterDescription.id.IsValid();
        }
        
        public static void StartAndRelease(this EventInstance instance)
        {
            instance.start();
            instance.release();
        }
        
        public static GameObject GetAttenuationObject(this StudioListener listener)
        {
            // FMOD integration code does not make this field public, so we have to use reflection to access it
            
            // Get the type of the StudioListener class
            var type = typeof(StudioListener);

            // Get the private field 'attenuationObject' using reflection
            var field = type.GetField("attenuationObject", 
                System.Reflection.BindingFlags.NonPublic 
                | System.Reflection.BindingFlags.Instance);

            // Get the value of the 'attenuationObject' field for the given listener instance
            var attenuationObject = field?.GetValue(listener) as GameObject;

            return attenuationObject;
        }
        
        public static void SetAttenuationObject(this StudioListener listener, GameObject newAttenuationObject)
        {
            // FMOD integration code does not make this field public, so we have to use reflection to access it
            
            // Get the type of the StudioListener class
            var type = typeof(StudioListener);

            // Get the private field 'attenuationObject' using reflection
            var field = type.GetField("attenuationObject", 
                System.Reflection.BindingFlags.NonPublic 
                | System.Reflection.BindingFlags.Instance);

            // Set the value of the 'attenuationObject' field for the given listener instance
            field?.SetValue(listener, newAttenuationObject);
        }
    }
}
