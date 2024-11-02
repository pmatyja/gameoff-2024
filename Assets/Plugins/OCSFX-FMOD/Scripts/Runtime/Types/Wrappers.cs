using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

namespace OCSFX.FMOD.Types
{
    [Serializable]
    public class FMODGlobalParameter
    {
        [SerializeField, ParamRef] private string _parameter;
        [SerializeField] private float _value;
        private PARAMETER_ID _id;

        public string Parameter
        {
            get => _parameter;
            set => _parameter = value;
        }

        public float Value
        {
            get => _value;
            set => _value = value;
        }

        public PARAMETER_ID ID
        {
            get => _id;
            set => _id = value;
        }

        public FMODGlobalParameter(string parameter, float value, PARAMETER_ID id = default)
        {
            _parameter = parameter;
            _value = value;
            _id = id;
        }
    }
    
    [Serializable]
    public class FMODParameter
    {
        [SerializeField] private string _parameter;
        [SerializeField] private float _value;
        private PARAMETER_ID _id;

        public string Parameter
        {
            get => _parameter;
            set => _parameter = value;
        }

        public float Value
        {
            get => _value;
            set => _value = value;
        }
        
        public PARAMETER_ID ID
        {
            get => _id;
            set => _id = value;
        }

        public FMODParameter(string parameter, float value, PARAMETER_ID id = default)
        {
            _parameter = parameter;
            _value = value;
            _id = id;
        }
    }

    [Serializable]
    public class FMODEvent
    {
        [SerializeField] private string _name;
        [SerializeField] private EventReference _eventRef;

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public EventReference EventRef
        {
            get => _eventRef;
            set => _eventRef = value;
        }
        
        public FMODEvent(string name, EventReference eventRef)
        {
            _name = name;
            _eventRef = eventRef;
        }
    }
    
    [Serializable]
    public class FMODBank
    {
        [SerializeField, BankRef] private string _bank;
        
        public string Bank
        {
            get => _bank;
            set => _bank = value;
        }
    }
    
    // [Serializable]
    // public class SceneUnityEvent
    // {
    //     [SerializeField] private string _sceneName;
    //     [SerializeField] private UnityEvent _onSceneLoaded;
    //     [SerializeField] private UnityEvent _onSceneUnloaded;
    //
    //     public string SceneName => _sceneName;
    //     public UnityEvent OnSceneLoaded => _onSceneLoaded;
    //     public UnityEvent OnSceneUnloaded => _onSceneUnloaded;
    // }
}