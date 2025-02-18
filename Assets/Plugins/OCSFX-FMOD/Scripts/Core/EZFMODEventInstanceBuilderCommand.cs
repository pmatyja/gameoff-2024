using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace OCSFX.EZFMOD
{
    internal abstract class EZFMODEventInstanceBuilderCommand
    {
        protected readonly EZFMODEventInstanceBuilder _builder;

        protected EZFMODEventInstanceBuilderCommand(EZFMODEventInstanceBuilder builder)
        {
            _builder = builder;
        }
        
        public abstract void Execute();
    }
    
    internal class EZFMODEventInstanceCreateInstanceCommand : EZFMODEventInstanceBuilderCommand
    {
        public EZFMODEventInstanceCreateInstanceCommand(EZFMODEventInstanceBuilder builder) : base(builder)
        {
        }

        public override void Execute()
        {
            _builder.CreateEventInstance();
        }
    }
    
    internal class EZFMODEventInstanceAttachToCommand : EZFMODEventInstanceBuilderCommand
    {
        private GameObject _attachObject;
        
        public EZFMODEventInstanceAttachToCommand(EZFMODEventInstanceBuilder builder, GameObject attachObject) : base(builder)
        {
            _attachObject = attachObject;
        }

        public override void Execute()
        {
            RuntimeManager.AttachInstanceToGameObject(_builder.EventInstance, _attachObject.transform);
        }
    }

    internal class EZFMODEventInstanceSetParameterByIDCommand : EZFMODEventInstanceBuilderCommand
    {
        private PARAMETER_ID _parameterID;
        private float _parameterValue;
        
        public EZFMODEventInstanceSetParameterByIDCommand(EZFMODEventInstanceBuilder builder, PARAMETER_ID parameterID, float parameterValue) : base(builder)
        {
            _parameterID = parameterID;
            _parameterValue = parameterValue;
        }

        public override void Execute()
        {
            _builder.EventInstance.setParameterByID(_parameterID, _parameterValue);
        }
    }
    
    internal class EZFMODEventInstanceSetParametersByIDsCommand : EZFMODEventInstanceBuilderCommand
    {
        private KeyValuePair<PARAMETER_ID, float>[] _parameterValuePairs;
        
        public EZFMODEventInstanceSetParametersByIDsCommand(EZFMODEventInstanceBuilder builder, KeyValuePair<PARAMETER_ID, float>[] parameterValuePairs) : base(builder)
        {
            _parameterValuePairs = parameterValuePairs;
        }

        public override void Execute()
        {
            foreach (var parameterValuePair in _parameterValuePairs)
            {
                _builder.EventInstance.setParameterByID(parameterValuePair.Key, parameterValuePair.Value);
            }
        }
    }
    
    internal class EZFMODEventInstanceSetParameterByNameCommand : EZFMODEventInstanceBuilderCommand
    {
        private string _parameterName;
        private float _parameterValue;
        
        public EZFMODEventInstanceSetParameterByNameCommand(EZFMODEventInstanceBuilder builder, string parameterName, float parameterValue) : base(builder)
        {
            _parameterName = parameterName;
            _parameterValue = parameterValue;
        }

        public override void Execute()
        {
            _builder.EventInstance.setParameterByName(_parameterName, _parameterValue);
        }
    }
    
    internal class EZFMODEventInstanceSetParametersByNamesCommand : EZFMODEventInstanceBuilderCommand
    {
        private KeyValuePair<string, float>[] _parameterValuePairs;
        
        public EZFMODEventInstanceSetParametersByNamesCommand(EZFMODEventInstanceBuilder builder, KeyValuePair<string, float>[] parameterValuePairs) : base(builder)
        {
            _parameterValuePairs = parameterValuePairs;
        }
        
        public EZFMODEventInstanceSetParametersByNamesCommand(EZFMODEventInstanceBuilder builder, List<KeyValuePair<string, float>> parameterValuePairs) : base(builder)
        {
            _parameterValuePairs = parameterValuePairs.ToArray();
        }
        
        public EZFMODEventInstanceSetParametersByNamesCommand(EZFMODEventInstanceBuilder builder, Dictionary<string, float> parameterValuePairs) : base(builder)
        {
            _parameterValuePairs = parameterValuePairs.ToArray();
        }

        public override void Execute()
        {
            foreach (var parameterValuePair in _parameterValuePairs)
            {
                _builder.EventInstance.setParameterByName(parameterValuePair.Key, parameterValuePair.Value);
            }
        }
    }
    
    
}