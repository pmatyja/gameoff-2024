using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using OCSFX.EZFMOD.Debug;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace OCSFX.EZFMOD
{
    internal class EZFMODEventInstanceBuilder
    {
        private EventInstance _eventInstance;
        
        public bool EventInstanceIsValid => _eventInstance.isValid();
        
        private EZFMODEventInstanceBuilder(GUID eventGuid)
        {
            var eventDescription = RuntimeManager.GetEventDescription(eventGuid);
            if (!eventDescription.isValid())
            {
                OCSFXLogger.LogError($"Event description is not valid for GUID: {eventGuid}");
                _eventInstance = EZFMODRuntimeStatics.INVALID_EVENT_INSTANCE;
                return;
            }
            
            _eventInstance = GetEventInstanceFromDescription(eventDescription);
        }
        
        private EZFMODEventInstanceBuilder(EventReference eventReference)
        {
            var eventDescription = RuntimeManager.GetEventDescription(eventReference);
            if (!eventDescription.isValid())
            {
                OCSFXLogger.LogError($"Event description is not valid for reference: {eventReference}");
                _eventInstance = EZFMODRuntimeStatics.INVALID_EVENT_INSTANCE;
                return;
            }
            
            _eventInstance = GetEventInstanceFromDescription(eventDescription);
        }
        private EZFMODEventInstanceBuilder(string eventStudioPath)
        {
            var eventDescription = RuntimeManager.GetEventDescription(eventStudioPath);
            if (!eventDescription.isValid())
            {
                OCSFXLogger.LogError($"Event description is not valid for path: {eventStudioPath}");
                _eventInstance = EZFMODRuntimeStatics.INVALID_EVENT_INSTANCE;
                return;
            }
            
            _eventInstance = GetEventInstanceFromDescription(eventDescription);
        }
        
        private EventInstance GetEventInstanceFromDescription(EventDescription eventDescription)
        {
            var result = eventDescription.createInstance(out var eventInstance);
            if (result != RESULT.OK)
            {
                OCSFXLogger.LogError($"Failed to create event instance from description: {eventDescription} with result: {result}");
            }
            
            return eventInstance;
        }
        
        public static EZFMODEventInstanceBuilder Create(GUID eventGuid) => new EZFMODEventInstanceBuilder(eventGuid);
        public static EZFMODEventInstanceBuilder Create(EventReference eventReference) => new EZFMODEventInstanceBuilder(eventReference);
        public static EZFMODEventInstanceBuilder Create(string eventPath) => new EZFMODEventInstanceBuilder(eventPath);
        
        public EZFMODEventInstanceBuilder SetParameterByID(PARAMETER_ID parameterID, float value)
        {
            if (!parameterID.IsValid()) return this;
            if (!_eventInstance.isValid()) return this;
            
            _eventInstance.setParameterByID(parameterID, value);
            return this;
        }
        
        public EZFMODEventInstanceBuilder SetParametersByIDs(KeyValuePair<PARAMETER_ID, float>[] parameterIdValuePairs)
        {
            if (!_eventInstance.isValid()) return this;
            
            foreach (var pair in parameterIdValuePairs)
            {
                SetParameterByID(pair.Key, pair.Value);
            }
            
            return this;
        }
        
        public EZFMODEventInstanceBuilder SetParameterByName(string parameterName, float value)
        {
            if (!_eventInstance.isValid()) return this;
            if (string.IsNullOrWhiteSpace(parameterName)) return this;
            
            _eventInstance.setParameterByName(parameterName, value);
            return this;
        }
        
        public EZFMODEventInstanceBuilder SetParametersByNames(KeyValuePair<string, float>[] parameterNameValuePairs)
        {
            if (!_eventInstance.isValid()) return this;
            foreach (var pair in parameterNameValuePairs)
            {
                SetParameterByName(pair.Key, pair.Value);
            }
            return this;
        }
        
        public EZFMODEventInstanceBuilder SetParametersByNames(Dictionary<string, float> parameterNameValuePairs)
        {
            if (!_eventInstance.isValid()) return this;
            foreach (var pair in parameterNameValuePairs)
            {
                SetParameterByName(pair.Key, pair.Value);
            }
            return this;
        }
        
        public EZFMODEventInstanceBuilder AttachTo(GameObject attachObject)
        {
            if (!_eventInstance.isValid()) return this;
            RuntimeManager.AttachInstanceToGameObject(_eventInstance, attachObject.transform);
            return this;
        }
        
        /* <summary>
         * Gets the event instance without starting it.
         * This is useful for handling any additional setup not provided by the builder.
         * </summary>
         */
        public EventInstance Build()
        {
            if (!_eventInstance.isValid())
            {
                OCSFXLogger.LogError($"[{nameof(EZFMODEventInstanceBuilder)}] Event instance is not valid.");
            }
            
            return EZFMODRuntimeStatics.INVALID_EVENT_INSTANCE;
        }
        
        /* <summary>
         * Starts the event instance. If keepPersistent is true, the instance will not be automatically released.
         * This is useful for recycling instances, but it can cause memory leaks if not handled properly.
         * So, by default, the instance is released after starting.
         * </summary>
         * <param name="keepPersistent">If true, the instance will not be automatically released. Be careful.</param>
         */
        public EventInstance BuildAndStart(bool keepPersistent = false)
        {
            if (!_eventInstance.isValid())
            {
                OCSFXLogger.LogError($"[{nameof(EZFMODEventInstanceBuilder)}] Event instance is not valid.");
                _eventInstance.release();
                return EZFMODRuntimeStatics.INVALID_EVENT_INSTANCE;
            }

            _eventInstance.start();
            if (!keepPersistent) _eventInstance.release();   
            
            return _eventInstance;
        }
    }
}