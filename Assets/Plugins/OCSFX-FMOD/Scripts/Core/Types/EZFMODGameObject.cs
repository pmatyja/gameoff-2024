using System;
using System.Collections.Generic;
using System.Linq;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using OCSFX.EZFMOD.Debug;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace OCSFX.EZFMOD.Types
{
    [AddComponentMenu(EZFMODRuntimeStatics.CREATE_COMPONENT_MENU_BASE + nameof(EZFMODGameObject))]
    [DisallowMultipleComponent]
    public class EZFMODGameObject : MonoBehaviour
    {
        public EZFMODEventBase Event;
        public bool PlayOnStart;
        public bool StopOnDestroy = true;
        
        private readonly Dictionary<IntPtr, EventInstance> _attachedInstances = new Dictionary<IntPtr, EventInstance>();
        private readonly Dictionary<string, float> _parameters = new Dictionary<string, float>();

        protected virtual void Awake()
        {
            EZFMODGameObjectManager.RegisterEZFMODGameObject(this);
        }

        protected virtual void Start()
        {
            StartWhenBanksReady();
        }

        private void PlayEventAtStart()
        {
            if (!Event || !PlayOnStart) return;
            PlayEvent(Event);
        }

        private void StartWhenBanksReady()
        {
            if (EZFMODRuntimeStatics.StartupBanksLoaded) PlayEventAtStart();
            else EZFMODRuntimeStatics.OnStartupBanksLoaded += OnMasterBanksLoaded;
        }
        
        private void OnMasterBanksLoaded()
        {
            EZFMODRuntimeStatics.OnStartupBanksLoaded -= StartWhenBanksReady;
            PlayEventAtStart();
        }
        
        [ContextMenu(nameof(PlayAssignedEvent))]
        public void PlayAssignedEvent()
        {
            if (!Event) return;
            
            PlayEvent(Event);
        }
        
#if UNITY_EDITOR
        [ContextMenu(nameof(Stop))]
        private void EditorStop() => Stop();
#endif
        public void Stop(bool allowFadeout = false)
        {
            foreach (var eventInstance in _attachedInstances.Values)
            {
                if (!eventInstance.isValid()) continue;
                        
                var stopMode = allowFadeout ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE;
                        
                eventInstance.stop(stopMode);
                eventInstance.release();
            }
                    
            _attachedInstances.Clear();
        }

#region Play Events via EZFMODEvent
        public EventInstance PlayEvent(EZFMODEvent ezfmodEvent, string parameter, float value)
        {
            var eventInstance = EZFMODEventInstanceBuilder.Create(ezfmodEvent.GUID)
                .AttachTo(gameObject)
                .SetParametersByNames(_parameters)
                .SetParameterByName(parameter, value)
                .BuildAndStart();
            
            AddToActiveInstances(eventInstance);
            SetParameter(parameter, value);
            
            return eventInstance;
        }

        public EventInstance PlayEvent(EZFMODEvent ezfmodEvent, PARAMETER_ID parameter, float value)
        {
            var eventInstance = EZFMODEventInstanceBuilder.Create(ezfmodEvent.GUID)
                .AttachTo(gameObject)
                .SetParametersByNames(_parameters)
                .SetParameterByID(parameter, value)
                .BuildAndStart();
            
            AddToActiveInstances(eventInstance);
            SetParameter(parameter, value);
            
            return eventInstance;
        }
        
        public void PlayEvent(EZFMODEventBase ezfmodEvent, out EventInstance eventInstance)
        {
            eventInstance = EZFMODEventInstanceBuilder.Create(ezfmodEvent.GUID)
                .SetParametersByNames(_parameters)
                .AttachTo(gameObject)
                .BuildAndStart();
            
            AddToActiveInstances(eventInstance);
        }
        
        public void PlayEvent(EZFMODEventBase ezfmodEvent)
        {
            PlayEvent(ezfmodEvent, out _);
        }
#endregion

#region Play Events via GUID
        public EventInstance PlayEvent(GUID eventGUID, string parameter, float value)
        {
            var eventInstance = EZFMODEventInstanceBuilder.Create(eventGUID)
                .SetParametersByNames(_parameters)
                .SetParameterByName(parameter, value)
                .AttachTo(gameObject)
                .BuildAndStart();
            
            AddToActiveInstances(eventInstance);
            SetParameter(parameter, value);
            
            return eventInstance;
        }
        
        public EventInstance PlayEvent(GUID eventGUID, PARAMETER_ID parameter, float value)
        {
            var eventInstance = EZFMODEventInstanceBuilder.Create(eventGUID)
                .SetParametersByNames(_parameters)
                .SetParameterByID(parameter, value)
                .AttachTo(gameObject)
                .BuildAndStart();
            
            AddToActiveInstances(eventInstance);
            SetParameter(parameter, value);
            
            return eventInstance;
        }
        
        public void PlayEvent(GUID eventGUID, out EventInstance eventInstance)
        {
            eventInstance = EZFMODEventInstanceBuilder.Create(eventGUID)
                .SetParametersByNames(_parameters)
                .AttachTo(gameObject)
                .BuildAndStart();
            
            AddToActiveInstances(eventInstance);
        }
        
        public void PlayEvent(GUID eventGUID)
        {
            PlayEvent(eventGUID, out _);
        }
#endregion

#region Play Events via EventReference
        public EventInstance PlayEvent(EventReference eventRef, string parameter, float value)
        {
            var eventInstance = EZFMODEventInstanceBuilder.Create(eventRef)
                .SetParametersByNames(_parameters)
                .SetParameterByName(parameter, value)
                .AttachTo(gameObject)
                .Build();
                    
            AddToActiveInstances(eventInstance);
            SetParameter(parameter, value);
                    
            return eventInstance;
        }
                
        public EventInstance PlayEvent(EventReference eventRef, PARAMETER_ID parameterID, float value)
        {
            var eventInstance = EZFMODEventInstanceBuilder.Create(eventRef)
                .SetParametersByNames(_parameters)
                .SetParameterByID(parameterID, value)
                .AttachTo(gameObject)
                .BuildAndStart();
                    
            AddToActiveInstances(eventInstance);
            SetParameter(parameterID, value);
                    
            return eventInstance;
        }

        public void PlayEvent(EventReference eventRef, out EventInstance eventInstance)
        {
            eventInstance = EZFMODEventInstanceBuilder.Create(eventRef)
                .SetParametersByNames(_parameters)
                .AttachTo(gameObject)
                .BuildAndStart();
                    
            AddToActiveInstances(eventInstance);
        }

        public void PlayEvent(EventReference eventRef)
        {
            PlayEvent(eventRef, out _);
        }
#endregion

#region Play Events via StudioPath
        public EventInstance PlayEvent(string eventPath, string parameter , float value)
        {
            var eventInstance = EZFMODEventInstanceBuilder.Create(eventPath)
                .SetParametersByNames(_parameters)
                .SetParameterByName(parameter, value)
                .AttachTo(gameObject)
                .BuildAndStart();
                    
            AddToActiveInstances(eventInstance);
            SetParameter(parameter, value);
                    
            return eventInstance;
        }
                
        public EventInstance PlayEvent(string eventPath, PARAMETER_ID parameterID, float value)
        {
            var newEventInstance = EZFMODEventInstanceBuilder.Create(eventPath)
                .SetParametersByNames(_parameters)
                .SetParameterByID(parameterID, value)
                .AttachTo(gameObject)
                .BuildAndStart();
                    
            AddToActiveInstances(newEventInstance);
            SetParameter(parameterID, value);
                    
            return newEventInstance;
        }

        public void PlayEvent(string eventPath, out EventInstance eventInstance)
        {
            eventInstance = EZFMODEventInstanceBuilder.Create(eventPath)
                .SetParametersByNames(_parameters)
                .AttachTo(gameObject)
                .BuildAndStart();
                    
            AddToActiveInstances(eventInstance);
        }

        public void PlayEvent(string eventPath)
        {
            PlayEvent(eventPath, out _);
        }
#endregion

#region Stop Events
        public void StopEvent(EventReference eventRef, bool allowFadeout = true)
        {
            if (eventRef.IsNull) return;

            var eventDesc = RuntimeManager.GetEventDescription(eventRef);
            eventDesc.getInstanceList(out var instanceList);
            
            foreach (var eventInstance in instanceList)
            {
                TryStopAttachedInstance(eventInstance, allowFadeout);
            }
        }
        
        public void StopEvent(string eventPath, bool allowFadeout = true)
        {
            if (string.IsNullOrWhiteSpace(eventPath)) return;

            var eventDesc = RuntimeManager.GetEventDescription(eventPath);
            eventDesc.getInstanceList(out var instanceList);
            
            foreach (var eventInstance in instanceList)
            {
                TryStopAttachedInstance(eventInstance, allowFadeout);
            }
        }

        public void StopEvent(GUID eventGUID, bool allowFadeout = true)
        {
            if (eventGUID.IsNull) return;

            var eventDesc = RuntimeManager.GetEventDescription(eventGUID);
            eventDesc.getInstanceList(out var instanceList);
            
            foreach (var eventInstance in instanceList)
            {
                TryStopAttachedInstance(eventInstance, allowFadeout);
            }
        }
#endregion

#region Parameters
        public void SetParameter(string parameterName, float value)
        {
            if (string.IsNullOrWhiteSpace(parameterName)) return;
            
            foreach (var eventInstance in _attachedInstances.Values)
            {
                if (!eventInstance.isValid()) continue;
                eventInstance.setParameterByName(parameterName, value);
            }
            
            AddParameter(parameterName, value);
        }
        
        public void SetParameter(PARAMETER_ID parameterID, float value)
        {
            if (!parameterID.IsValid()) return;
            
            PARAMETER_DESCRIPTION paramDesc = default;
            
            foreach (var eventInstance in _attachedInstances.Values.Where(eventInstance => eventInstance.isValid()))
            {
                if (eventInstance.getDescription(out var eventDesc) != RESULT.OK)
                {
                    OCSFXLogger.LogError($"Failed to get event description for instance {eventInstance.handle}.", this);
                    continue;
                }

                if (!paramDesc.IsValid() && eventDesc.getParameterDescriptionByID(parameterID, out paramDesc) != RESULT.OK)
                {
                    OCSFXLogger.LogError($"Failed to get parameter description for parameter ID {parameterID}.", this);
                    continue;
                }

                eventInstance.setParameterByID(paramDesc.id, value);
            }

            if (!paramDesc.IsValid()) return;
            
            AddParameter(paramDesc.name, value);
        }
        
        private void AddParameter(string parameterName, float value)
        {
            if (!_parameters.TryAdd(parameterName, value)) _parameters[parameterName] = value;
            
            ApplyParameters();
        }

        public float GetParameterValue(string parameterName)
        {
            _parameters.TryGetValue(parameterName, out var value);
            
            return value;
        }
        
        public float GetParameterValue(PARAMETER_ID parameterID)
        {
            PARAMETER_DESCRIPTION paramDesc = default;
            
            foreach (var eventInstance in _attachedInstances.Values.Where(eventInstance => eventInstance.isValid()))
            {
                if (eventInstance.getDescription(out var eventDesc) != RESULT.OK)
                {
                    OCSFXLogger.LogError($"Failed to get event description for instance {eventInstance.handle}.", this);
                    continue;
                }

                if (!paramDesc.IsValid() && eventDesc.getParameterDescriptionByID(parameterID, out paramDesc) != RESULT.OK)
                {
                    OCSFXLogger.LogError($"Failed to get parameter description for parameter ID {parameterID}.", this);
                }
            }

            if (!paramDesc.IsValid()) return 0;
            
            return GetParameterValue(paramDesc.name);
        }

        private void ApplyParameters()
        {
            foreach (var eventInstance in _attachedInstances.Values)
            {
                if (!EZFMODRuntimeStatics.IsInstanceActive(eventInstance)) continue;
                
                foreach (var (paramKey, paramValue) in _parameters)
                {
                    eventInstance.setParameterByName(paramKey, paramValue);
                }
            }
        }

#endregion
        
        private void AddToActiveInstances(EventInstance eventInstance)
        {
            if (!eventInstance.isValid()) return;
            
            _attachedInstances.TryAdd(eventInstance.handle, eventInstance);
        }
        
        private bool TryStopAttachedInstance(EventInstance eventInstance, bool allowFadeout = true)
        {
            if (!eventInstance.isValid()) return false;
            if (!_attachedInstances.TryGetValue(eventInstance.handle, out var attachedInstance)) return false;
            
            attachedInstance.Stop(allowFadeout);
                
            _attachedInstances.Remove(eventInstance.handle);
            
            return true;
        }

        internal void CleanUpDeadInstances()
        {
            var keysToRemove = new List<IntPtr>();

            foreach (var pair in _attachedInstances)
            {
                if (!pair.Value.isValid())
                {
                    keysToRemove.Add(pair.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                _attachedInstances.Remove(key);
            }
        }

        protected virtual void OnDestroy()
        {
            if (StopOnDestroy) Stop();
            
            EZFMODGameObjectManager.UnregisterEZFMODGameObject(this);
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if (!Event) return;

            DrawAttenuationRadiusGizmos();
        }

        private void DrawAttenuationRadiusGizmos()
        {
            if (Event is not EZFMODEvent ezfmodEvent) return;
            if (!ezfmodEvent.Is3D) return;
            
            var currentGizmoColor = Gizmos.color;
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, ezfmodEvent.MinDistance);

            Gizmos.color *= 0.5f;
            Gizmos.DrawWireSphere(transform.position, ezfmodEvent.MaxDistance);

            Gizmos.color *= 0.5f;
            Gizmos.DrawSphere(transform.position, ezfmodEvent.MaxDistance);
            
            // Restore color
            Gizmos.color = currentGizmoColor;
        }
    }
}