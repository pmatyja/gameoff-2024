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
        private readonly GUID _eventGuid;
        private readonly string _eventPath;
        
        public bool EventInstanceIsValid => _eventInstance.isValid();

        private readonly Queue<EZFMODEventInstanceBuilderCommand> _commands;
        
        private EZFMODEventInstanceBuilder(GUID eventGuid)
        {
            _eventGuid = eventGuid;

            _commands = new Queue<EZFMODEventInstanceBuilderCommand>();
            
            var command = new EZFMODEventInstanceCreateInstanceCommand(this);
            _commands.Enqueue(command);
        }
        
        private EZFMODEventInstanceBuilder(EventReference eventReference)
        {
            _eventPath = eventReference.ToString();
            _eventGuid = eventReference.Guid;
            
            _commands = new Queue<EZFMODEventInstanceBuilderCommand>();
            
            var command = new EZFMODEventInstanceCreateInstanceCommand(this);
            _commands.Enqueue(command);
        }
        private EZFMODEventInstanceBuilder(string eventStudioPath)
        {
            _eventPath = eventStudioPath;
            
            _commands = new Queue<EZFMODEventInstanceBuilderCommand>();
            
            var command = new EZFMODEventInstanceCreateInstanceCommand(this);
            _commands.Enqueue(command);
        }
        
        internal void CreateEventInstance()
        {
            EventDescription eventDescription;
            
            if (_eventGuid != EZFMODRuntimeStatics.INVALID_GUID)
            {
                eventDescription = RuntimeManager.GetEventDescription(_eventGuid);
            }
            else if (!string.IsNullOrWhiteSpace(_eventPath))
            {
                eventDescription = RuntimeManager.GetEventDescription(_eventPath);
            }
            else
            {
                OCSFXLogger.LogError($"[{nameof(EZFMODEventInstanceBuilder)}] Event GUID and Event Path are both invalid.");
                return;
            }
            
            _eventInstance = GetEventInstanceFromDescription(eventDescription);
        }
        
        private static EventInstance GetEventInstanceFromDescription(EventDescription eventDescription)
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
            var command = new EZFMODEventInstanceSetParameterByIDCommand(this, parameterID, value);
            _commands.Enqueue(command);
            return this;
        }
        
        public EZFMODEventInstanceBuilder SetParametersByIDs(KeyValuePair<PARAMETER_ID, float>[] parameterIdValuePairs)
        {
            var command = new EZFMODEventInstanceSetParametersByIDsCommand(this, parameterIdValuePairs);
            _commands.Enqueue(command);
            return this;
        }
        
        public EZFMODEventInstanceBuilder SetParameterByName(string parameterName, float value)
        {
            var command = new EZFMODEventInstanceSetParameterByNameCommand(this, parameterName, value);
            _commands.Enqueue(command);
            return this;
        }
        
        public EZFMODEventInstanceBuilder SetParametersByNames(KeyValuePair<string, float>[] parameterNameValuePairs)
        {
            var command = new EZFMODEventInstanceSetParametersByNamesCommand(this, parameterNameValuePairs);
            _commands.Enqueue(command);
            return this;
        }
        
        public EZFMODEventInstanceBuilder SetParametersByNames(Dictionary<string, float> parameterNameValuePairs)
        {
            var command = new EZFMODEventInstanceSetParametersByNamesCommand(this, parameterNameValuePairs);
            _commands.Enqueue(command);
            return this;
        }
        
        public EZFMODEventInstanceBuilder AttachTo(GameObject attachObject)
        {
            var command = new EZFMODEventInstanceAttachToCommand(this, attachObject);
            _commands.Enqueue(command);
            return this;
        }
        
        private void ExecuteCommands()
        {
            while (_commands.Count > 0)
            {
                var command = _commands.Dequeue();
                command.Execute();
            }
        }
        
        /* <summary>
         * Gets the event instance without starting it.
         * This is useful for handling any additional setup not provided by the builder.
         * </summary>
         */
        public EventInstance Build()
        {
            ExecuteCommands();
            
            if (!_eventInstance.isValid())
            {
                OCSFXLogger.LogError($"[{nameof(EZFMODEventInstanceBuilder)}] Event instance is not valid.");
                _eventInstance.release();
                return EZFMODRuntimeStatics.INVALID_EVENT_INSTANCE;
            }
            
            return _eventInstance;
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
            ExecuteCommands();
            
            if (!_eventInstance.isValid())
            {
                OCSFXLogger.LogError($"[{nameof(EZFMODEventInstanceBuilder)}] Event instance is not valid.");
                _eventInstance.release();
                return EZFMODRuntimeStatics.INVALID_EVENT_INSTANCE;
            }
            
            if (keepPersistent) return _eventInstance;
            
            var result = _eventInstance.start();
            if (result != RESULT.OK)
            {
                OCSFXLogger.LogError($"[{nameof(EZFMODEventInstanceBuilder)}] Failed to start event instance with result: {result}");
                _eventInstance.release();
                return EZFMODRuntimeStatics.INVALID_EVENT_INSTANCE;
            }
            
            _eventInstance.release();
            return EZFMODRuntimeStatics.INVALID_EVENT_INSTANCE;
        }
        
        internal EventInstance EventInstance => _eventInstance;
    }
}