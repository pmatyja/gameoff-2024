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

        public void Load() => RunCoroutine(Co_LoadBankWhenRuntimeManagerIsInitialized());
        
        public void Load(Action<EZFMODBank> callback) => RunCoroutine(Co_LoadBankWhenRuntimeManagerIsInitialized(false, callback));

        public void LoadWithSampleData() => RunCoroutine(Co_LoadBankWhenRuntimeManagerIsInitialized(true));
        
        public void LoadWithSampleData(Action<EZFMODBank> callback) => RunCoroutine(Co_LoadBankWhenRuntimeManagerIsInitialized(true, callback));
        
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
        
        private void RunCoroutine(IEnumerator coroutine) => EZFMODRuntimeStatics.RunCoroutine(coroutine);
        
        private IEnumerator Co_LoadBankWhenRuntimeManagerIsInitialized(bool loadSamples = false, Action<EZFMODBank> callback = null)
        {
            yield return Co_YieldForRuntimeManager();
            if (loadSamples)
            {
                LoadWithSampleData(callback);
            }
            else
            {
                Load(callback);
            }
        }

        private IEnumerator Co_YieldForRuntimeManager()
        {
            while (!RuntimeManager.IsInitialized)
            {
                yield return null;
            }
        }
    }
}
