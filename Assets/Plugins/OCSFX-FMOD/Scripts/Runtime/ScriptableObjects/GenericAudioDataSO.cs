using System.Collections.Generic;
using FMODUnity;
using OCSFX.FMOD.Types;
using UnityEngine;

namespace OCSFX.FMOD.AudioData
{
    [CreateAssetMenu(menuName = _CREATE_ASSET_MENU_BASE + "Generic", fileName = nameof(GenericAudioDataSO))]
    public class GenericAudioDataSO: AudioDataSO
    {
        [SerializeField] protected List<FMODBank> _banks = new List<FMODBank>();
        [SerializeField] protected List<FMODParameter> _parameters = new List<FMODParameter>();
        [SerializeField] protected List<FMODGlobalParameter> _globalParameters = new List<FMODGlobalParameter>();
        [SerializeField] protected List<FMODEvent> _events = new List<FMODEvent>();
        
        public List<FMODBank> Banks => _banks;
        public List<FMODParameter> Parameters => _parameters;
        public List<FMODGlobalParameter> GlobalParameters => _globalParameters;
        public List<FMODEvent> Events => _events;

        public EventReference GetEventRef(string eventStructName)
        {
            var eventStruct = _events.Find(eventStruct => eventStruct.Name == eventStructName);

            return eventStruct.EventRef;
        }

        public FMODParameter GetParameter(string paramName)
        {
            return _parameters.Find(param => param.Parameter == paramName);
        }

        public void LoadBank(string bank)
        {
            var bankStruct = _banks.Find(bankStruct => bankStruct.Bank == bank);

            if (string.IsNullOrWhiteSpace(bankStruct.Bank)) return;
            
            RuntimeManager.LoadBank(bankStruct.Bank);
        }

        public void UnloadBank(string bank)
        {
            var bankStruct = _banks.Find(bankStruct => bankStruct.Bank == bank);

            if (string.IsNullOrWhiteSpace(bankStruct.Bank)) return;
            
            RuntimeManager.UnloadBank(bankStruct.Bank);
        }

        public void LoadBanks()
        {
            foreach (var bankStruct in _banks)
            {
                if (string.IsNullOrWhiteSpace(bankStruct.Bank)) continue;
            
                RuntimeManager.LoadBank(bankStruct.Bank);
            }
        }

        public void UnloadBanks()
        {
            foreach (var bankStruct in _banks)
            {
                if (string.IsNullOrWhiteSpace(bankStruct.Bank)) continue;
            
                RuntimeManager.UnloadBank(bankStruct.Bank);
            }
        }
    }
}