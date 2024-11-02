using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OCSFX.FMOD.Prototype
{
    public class CustomUnityEventTester : MonoBehaviour
    {
        [SerializeField] private List<StringParameterValueStruct> _stringParamValueStructs = new();

        [Header("UnityEvents")]
        [SerializeField] private List<UnityEvent> _unityEvents;
        [SerializeField] private List<NamedUnityEvent> _namedUnityEvents;

        [ContextMenu("Invoke All Unity Events")]
        public void InvokeUnityEvents()
        {
            foreach (var unityEvent in _unityEvents) unityEvent?.Invoke();
        }

        private void StringParameterValueMethod(StringParameterValueStruct theStruct)
        {
            Debug.Log(
                $"{nameof(theStruct.Action)}: {theStruct.Action} | " +
                $"{nameof(theStruct.Parameter)}: {theStruct.Parameter} | " +
                $"{nameof(theStruct.Value)}: {theStruct.Value}");
        }

        public void SetParameterValueByName(string theName)
        {
            var theStruct = _stringParamValueStructs.Find(foundStruct => foundStruct.Action == theName);
            if (string.IsNullOrWhiteSpace(theStruct.Action)) return;
            if (string.IsNullOrWhiteSpace(theStruct.Parameter)) return;
            
            StringParameterValueMethod(theStruct);
        }

        public void InvokeUnityEvent(string eventName)
        {
            var foundUnityEvent = _namedUnityEvents.Find(checkedEvent => checkedEvent.Name == eventName);

            if (string.IsNullOrWhiteSpace(foundUnityEvent.Name)) return;
            
            Debug.Log($"UnityEvent: {foundUnityEvent.Name}");
            foundUnityEvent.Event?.Invoke();
        }
    }

    // Structs

    [Serializable]
    public struct StringStringStruct
    {
        public string String1;
        public string String2;
    }

    [Serializable]
    public struct StringFloatStruct
    {
        public string TheString;
        public float TheFloat;
    }

    [Serializable]
    public struct StringParameterValueStruct
    {
        public string Action;
        public string Parameter;
        public float Value;
    }

    // Struct to enable named UnityEvent list elements
    [Serializable]
    public struct NamedUnityEvent
    {
        public string Name;
        public UnityEvent Event;
    }
}
