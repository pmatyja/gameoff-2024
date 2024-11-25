using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace OCSFX.FMOD
{
    [AddComponentMenu(OCSFXAudioStatics.CREATE_COMPONENT_MENU_BASE + nameof(FMODGameObject))]
    public class FMODGameObject : MonoBehaviour
    {
        private readonly HashSet<EventInstance> _attachedInstances = new HashSet<EventInstance>();

        private readonly Dictionary<string, float> _parameters = new Dictionary<string, float>();

        public HashSet<EventInstance> AttachedInstances => _attachedInstances;

        public EventInstance PlayEvent(EventReference eventPath, string parameter = default, float value = default)
        {
            if (eventPath.IsNull) return new EventInstance();
            
            var eventDesc = RuntimeManager.GetEventDescription(eventPath);
            
            eventDesc.createInstance(out var newInstance);

            RuntimeManager.AttachInstanceToGameObject(newInstance, gameObject.transform);
            _attachedInstances.Add(newInstance);

            newInstance.start();
            
            if (parameter != default)
                SetParameter(parameter, value);
            else ApplyParameters();
            
            newInstance.release();

            //Debug.Log(_attachedInstances.Count);
            return newInstance;
        }
        
        public EventInstance PlayEvent(string eventPath, string parameter = default, float value = default)
        {
            if (string.IsNullOrWhiteSpace(eventPath)) return new EventInstance();
            
            var eventDesc = RuntimeManager.GetEventDescription(eventPath);
            
            eventDesc.createInstance(out var newInstance);

            RuntimeManager.AttachInstanceToGameObject(newInstance, gameObject.transform);
            _attachedInstances.Add(newInstance);

            newInstance.start();
            
            if (parameter != default)
                SetParameter(parameter, value);
            else ApplyParameters();
            
            newInstance.release();

            //Debug.Log(_attachedInstances.Count);
            return newInstance;
        }

        public void StopEvent(EventReference eventRef, bool allowFadeout = true)
        {
            if (eventRef.IsNull) return;

            var eventDesc = RuntimeManager.GetEventDescription(eventRef);
            eventDesc.getInstanceList(out var instanceList);

            if (instanceList.Length < 1) return;
            
            var matchingInstances = new List<EventInstance>();

            // TODO: Optimize this. We can't use "Contains" in the hashset because event instance equality is not reliable.
            foreach (var eventInstance in instanceList)
            {
                if (!eventInstance.isValid()) continue;
                
                var instanceHandle = eventInstance.handle;
                
                var matchingInstance = _attachedInstances.FirstOrDefault(i => i.handle == instanceHandle);
                if (!matchingInstance.isValid()) continue;

                matchingInstances.Add(eventInstance);
            }

            foreach (var instance in matchingInstances)
            {
                instance.Stop();
            }
        }
        
        public void StopEvent(string eventPath, bool allowFadeout = true)
        {
            if (string.IsNullOrWhiteSpace(eventPath)) return;

            var eventDesc = RuntimeManager.GetEventDescription(eventPath);
            eventDesc.getInstanceList(out var instanceList);

            if (instanceList.Length < 1) return;
            
            var matchingInstances = new List<EventInstance>();

            // TODO: Optimize this. We can't use "Contains" in the hashset because event instance equality is not reliable.
            foreach (var eventInstance in instanceList)
            {
                if (!eventInstance.isValid()) continue;
                
                var instanceHandle = eventInstance.handle;
                
                var matchingInstance = _attachedInstances.FirstOrDefault(i => i.handle == instanceHandle);
                if (!matchingInstance.isValid()) continue;

                matchingInstances.Add(eventInstance);
            }

            foreach (var instance in matchingInstances)
            {
                instance.Stop(allowFadeout);
            }
        }

        public void Stop(bool allowFadeout = false)
        {
            foreach (var eventInstance in _attachedInstances)
            {
                var stopMode = allowFadeout ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE;
                
                eventInstance.stop(stopMode);
                eventInstance.release();
            }
        }

        public void SetParameter(EventReference eventRef, string parameterName, float value)
        {
            var eventDesc = RuntimeManager.GetEventDescription(eventRef);
            
            eventDesc.getParameterDescriptionByName(parameterName, out var paramDesc);
            eventDesc.getInstanceList(out var instanceList);

            var parameterID = paramDesc.id;

            var activeInstances = new List<EventInstance>();
            foreach (var eventInstance in instanceList)
            {
                if (!_attachedInstances.Contains(eventInstance)) continue;
                activeInstances.Add(eventInstance);
                break;
            }

            foreach (var eventInstance in activeInstances)
                eventInstance.setParameterByID(parameterID, value);
        }

        public void SetParameter(string parameterName, float value)
        {
            //Debug.Log("Set Parameter " + parameterName + " to " + value);
            if (!_parameters.TryAdd(parameterName, value)) _parameters[parameterName] = value;

            ApplyParameters();
        }

        public float GetParameterValue(string parameterName)
        {
            _parameters.TryGetValue(parameterName, out var value);
            
            return value;
        }

        private void ApplyParameters()
        {
            //Debug.Log("Apply Parameters");
            
            foreach (var eventInstance in _attachedInstances)
            {
                eventInstance.getPlaybackState(out var playbackState);
                switch (playbackState)
                {
                    case PLAYBACK_STATE.PLAYING:
                    case PLAYBACK_STATE.STARTING:
                    case PLAYBACK_STATE.SUSTAINING:
                    break;
                    case PLAYBACK_STATE.STOPPING:
                    case PLAYBACK_STATE.STOPPED:
                    default:
                    continue;
                }
                
                foreach (var (paramKey, paramValue) in _parameters)
                {
                    eventInstance.setParameterByName(paramKey, paramValue);
                    //Debug.Log("Set " + paramKey + " to " + paramValue);
                }
            }
        }

        private void LateUpdate()
        {
            // Consider making this an InvokeRepeating on a less-frequent tick.
            CleanUpDeadInstances();
        }

        private void CleanUpDeadInstances()
        {
            if (_attachedInstances.Count < 1) return;

            var toRemoveList = new List<EventInstance>();
            
            foreach (var instance in _attachedInstances)
            {
                if (instance.isValid()) continue;
                toRemoveList.Add(instance);
            }

            foreach (var instance in toRemoveList)
            {
                instance.release();
                _attachedInstances.Remove(instance);
            }
        }
    }
}