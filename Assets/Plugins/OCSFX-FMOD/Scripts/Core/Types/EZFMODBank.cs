using System;
using System.Collections;
using FMODUnity;
using OCSFX.EZFMOD.Attributes;
using UnityEngine;
using GUID = FMOD.GUID;

namespace OCSFX.EZFMOD.Types
{
    public class EZFMODBank : EZFMODAsset
    {
        [field: SerializeField, ReadOnly] public bool IsMasterBank { get; private set; }

        internal void Init(string inName, string studioPath, GUID guid, bool isMasterBank = false)
        {
            Name = inName;
            StudioPath = studioPath;
            GUID = guid;
            IsMasterBank = isMasterBank;
        }

        public void Load() => RuntimeManager.LoadBank(Name);
        
        public void Load(Action<EZFMODBank> callback) => RunCoroutine(Co_LoadWithCallback(callback, false));

        public void LoadWithSampleData() => RuntimeManager.LoadBank(Name, true);
        
        public void LoadWithSampleData(Action<EZFMODBank> callback) => RunCoroutine(Co_LoadWithCallback(callback, true));
        
        public void Unload() => RuntimeManager.UnloadBank(Name);

        public bool IsLoaded() => RuntimeManager.HasBankLoaded(Name);

        private IEnumerator Co_LoadWithCallback(Action<EZFMODBank> callback, bool loadSamples)
        {
            RuntimeManager.LoadBank(Name, loadSamples);
            while (!IsLoaded() || loadSamples && RuntimeManager.AnySampleDataLoading())
            {
                yield return null;
            }
            
            callback?.Invoke(this);
        }
        
        private void RunCoroutine(IEnumerator enumerator) => EZFMODRuntimeStatics.RunCoroutine(enumerator);
    }
}
