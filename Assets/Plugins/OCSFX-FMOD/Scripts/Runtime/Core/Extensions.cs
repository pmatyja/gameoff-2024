using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using OCSFX.FMOD.Types;
using UnityEngine;
using Debug = UnityEngine.Debug;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace OCSFX.FMOD
{
    public static class Extensions
    {
        // Helper
        private static readonly EventInstance _INVALID_FMOD_EVENT_INSTANCE = default;

        public static T GetOrAdd<T>(this GameObject attachGameObject) where T : Component
        {
            if (!attachGameObject.TryGetComponent<T>(out var component))
                component = attachGameObject.AddComponent<T>();

            return component;
        }
        
        public static void PlayOneShot(this EventReference eventRef, Vector3 position = default)
        {
            if (!RuntimeManager.IsInitialized) return;
            
            RuntimeManager.PlayOneShot(eventRef, position);
        }

        public static void PlayOneShotAttached(this EventReference eventRef, GameObject soundSource)
        {
            if (!RuntimeManager.IsInitialized) return;
            
            RuntimeManager.PlayOneShotAttached(eventRef, soundSource);
        }

        public static EventInstance Play(this EventReference eventRef, GameObject sourceObject, string parameter = default, float value = default)
        {
            if (!sourceObject) return _INVALID_FMOD_EVENT_INSTANCE;

            if (!sourceObject.TryGetComponent<FMODGameObject>(out var fmodGameObject)) 
                fmodGameObject = sourceObject.AddComponent<FMODGameObject>();

            return parameter != default ? fmodGameObject.PlayEvent(eventRef, parameter, value) : fmodGameObject.PlayEvent(eventRef);
        }

        public static EventInstance Play2D(this EventReference eventRef, string parameter = default, float value = default)
        {
            if (!RuntimeManager.IsInitialized) return _INVALID_FMOD_EVENT_INSTANCE;
            
            var newInstance = RuntimeManager.CreateInstance(eventRef);

            if (parameter != default) newInstance.setParameterByName(parameter, value);

            newInstance.getDescription(out var eventDesc);
            eventDesc.is3D(out var is3D);
            if (is3D)
            {
                var listener3dAttributes = Object.FindObjectOfType<StudioListener>().transform.To3DAttributes();
                newInstance.set3DAttributes(listener3dAttributes);
            }

            newInstance.start();
            newInstance.release();

            return newInstance;
        }
        
        public static void Stop(this EventReference eventRef, GameObject sourceObject, bool allowFadeout = true)
        {
            if (!sourceObject) return;
            
            if (!sourceObject.TryGetComponent<FMODGameObject>(out var fmodGameObject))
                fmodGameObject = sourceObject.AddComponent<FMODGameObject>();
                
            fmodGameObject.StopEvent(eventRef, allowFadeout);
        }
        
        public static void Stop2D(this EventReference eventRef, bool allowFadeout = true)
        {
            if (!RuntimeManager.IsInitialized) return;
            
            var eventDesc = RuntimeManager.GetEventDescription(eventRef);
            
            eventDesc.getInstanceList(out var instanceList);

            if (instanceList.Length < 1) return;

            var stopMode = allowFadeout ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE;

            instanceList[0].stop(stopMode);
            instanceList[0].release();
        }
        
        public static void StopGlobal(this EventReference eventRef, bool allowFadeout = true)
        {
            if (!RuntimeManager.IsInitialized) return;

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
        
        public static string GetEventName(this EventReference eventReference)
        {
            var eventPath = eventReference.ToString();

            var segments = eventPath.Split("/");
            var eventName = segments[^1];

            return eventName;
        }
        
        public static double GetDuration(this EventReference eventReference)
        {
            if (!RuntimeManager.IsInitialized) return 0.0;

            var eventDesc = RuntimeManager.GetEventDescription(eventReference);
            
            eventDesc.getLength(out var durationInMS);
            var durationInSeconds = durationInMS / 1000.0;
            
            return durationInSeconds;
        }
        
        public static double GetDuration(this EventInstance instance)
        {
            instance.getDescription(out var eventDesc);
            eventDesc.getLength(out var durationInMS);
            var durationInSeconds = durationInMS / 1000.0;
            
            return durationInSeconds;
        }
        
        public static void Stop(this EventInstance instance, bool allowFadeout = true)
        {
            if (!instance.isValid())
            {
                Debug.LogWarning(nameof(Extensions) + " tried to stop an invalid event instance.");
                return;
            }

            var stopMode = allowFadeout ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE;
            instance.stop(stopMode);
            instance.release();
        }

        public static string GetEventName(this EventInstance eventInstance)
        {
            eventInstance.getDescription(out var eventDescription);
            eventDescription.getPath(out var eventPath);

            var segments = eventPath.Split("/");
            var eventName = segments[^1];

            return eventName;
        }
        
        public static float GetEventVolume(this EventInstance eventInstance)
        {
            eventInstance.getVolume(out var volume);
            return volume;
        }

        public static void SetEventVolume(this EventInstance eventInstance, float multiplier = 1)
        {
            if (!eventInstance.isValid()) return;
            
            multiplier = Mathf.Clamp(multiplier, 0, 2);
            
            eventInstance.getVolume(out var instanceVolume);
            eventInstance.setVolume(multiplier * instanceVolume);
        }
        
        public static void SetParameter(this GameObject sourceObject, string parameterName, float value)
        {
            if (!sourceObject) return;
            
            if (!sourceObject.TryGetComponent<FMODGameObject>(out var fmodGameObject)){}
                fmodGameObject = sourceObject.AddComponent<FMODGameObject>();
            
            fmodGameObject.SetParameter(parameterName,value);
        }
        
        public static bool TryGetParameter(this List<FMODParameter> fmodParamStructs, string structName, out string parameter)
        {
            parameter = fmodParamStructs.GetParameter(structName);
            return !string.IsNullOrWhiteSpace(parameter);
        }

        public static string GetParameter(this List<FMODParameter> fmodParamStructs, string structName)
        {
            return fmodParamStructs.Find(fmodParamStruct => fmodParamStruct.Parameter == structName).Parameter;
        }
        
        public static bool TryGetParameter(this List<FMODGlobalParameter> fmodParamStructs, string structName, out string parameter)
        {
            parameter = fmodParamStructs.GetParameter(structName);
            return !string.IsNullOrWhiteSpace(parameter);
        }

        public static string GetParameter(this List<FMODGlobalParameter> fmodParamStructs, string structName)
        {
            return fmodParamStructs.Find(fmodParamStruct => fmodParamStruct.Parameter == structName).Parameter;
        }
        
        public static bool TryGetEventReference(this List<FMODEvent> fmodEventStructs, string structName, out EventReference eventReference)
        {
            eventReference = fmodEventStructs.GetEventReference(structName);
            return !eventReference.IsNull;
        }

        public static EventReference GetEventReference(this List<FMODEvent> fmodEventStructs, string structName)
        {
            return fmodEventStructs.Find(fmodEventStruct => fmodEventStruct.Name == structName).EventRef;
        }
        
        public static bool TryGetBank(this List<FMODBank> fmodBankStructs, string structName, out string bank)
        {
            bank = fmodBankStructs.GetBank(structName);
            return !string.IsNullOrWhiteSpace(bank);
        }
        
        public static string GetBank(this List<FMODBank> fmodBankStructs, string structName)
        {
            return fmodBankStructs.Find(fmodBankStruct => fmodBankStruct.Bank == structName).Bank;
        }
    }
}
