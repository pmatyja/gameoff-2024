using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using OCSFX.EZFMOD.Types;
using OCSFX.EZFMOD.Utility;
using OCSFX.EZFMOD.Debug;
using UnityEngine;

namespace OCSFX.EZFMOD.ScriptableObjects
{
    [CreateAssetMenu(menuName = _CREATE_ASSET_MENU_BASE + "Banks", fileName = nameof(EZFMODBanksAudioDataSO))]
    public class EZFMODBanksAudioDataSO : EZFMODAudioDataSO
    {
        [Space] [SerializeField] private List<EZFMODBank> _startupBanks = new();
        
        [SerializeField] [Tooltip("A minimum added delay to help ensure that startup banks have sufficient time to load.")]
        [Range(0f, 10f)] private float _startupBankPostLoadBuffer = 0.1f;

        [Space] [SerializeField] private List<EZFMODBank> _runtimeBanks = new ();

        // Properties
        public List<EZFMODBank> StartupBanks => _startupBanks;
        public List<EZFMODBank> RuntimeBanks => _runtimeBanks;

        public void LoadStartupBanks()
        {
            CoroutineRunner
                .CreateAndRun(Co_LoadStartupBanks(_startupBankPostLoadBuffer))
                .SetName("Bank Loader (Startup Banks)");
        }

        private IEnumerator Co_LoadStartupBanks(float postLoadBuffer = 0f)
        {
            if (EZFMODRuntimeStatics.StartupBanksLoaded) yield break;
            
            var startupBankLoadStartTime = Time.realtimeSinceStartup;
            
            OCSFXLogger.Log($"[{this}] Load Startup Banks - process started at {startupBankLoadStartTime} seconds", this, _showDebug);
        
            while (!RuntimeManager.HaveMasterBanksLoaded) yield return null;
            while (RuntimeManager.AnySampleDataLoading()) yield return null;
            OCSFXLogger.Log($"[{this}] Master Banks ready.", this, _showDebug);
            
            foreach (var bank in _startupBanks)
            {
                if (!bank) continue;
                
                bank.LoadWithSampleData();
            }

            foreach (var bank in _startupBanks)
            {
                while (!bank.IsLoaded() || RuntimeManager.AnySampleDataLoading())
                    yield return null;
                
                OCSFXLogger.Log($"[{this}] {bank} (Startup Bank) Loaded.",this, _showDebug);
            }
            
            yield return new WaitForSeconds(postLoadBuffer);

            var startupLoadFinishTime = Time.realtimeSinceStartup;
            
            OCSFXLogger.Log($"[{this}] Load Startup Banks - process completed at {startupLoadFinishTime} seconds", this, _showDebug);
            
            var startupLoadTime = startupLoadFinishTime - startupBankLoadStartTime;
            
            OCSFXLogger.Log($"[{this}] Startup Banks finished loading after {startupLoadTime} seconds", this, _showDebug);
            EZFMODRuntimeStatics.OnStartupBanksLoaded?.Invoke();
        }

        private IEnumerator Co_LoadRuntimeBank(string bank)
        {
            var loadStartTime = Time.realtimeSinceStartup;
            OCSFXLogger.Log($"[{this}] {bank} (Runtime Bank) Load - process started at {loadStartTime} seconds", this, _showDebug);
            RuntimeManager.LoadBank(bank, true);
            while (!RuntimeManager.HasBankLoaded(bank) || RuntimeManager.AnySampleDataLoading()) yield return null;

            var loadTime = Time.realtimeSinceStartup - loadStartTime;
            
            OCSFXLogger.Log($"[{this}] {bank} (Runtime Bank) Loaded after {loadTime} seconds", this, _showDebug);
        }

        private IEnumerator Co_UnloadRuntimeBank(string bank)
        {
            var unloadStartTime = Time.realtimeSinceStartup;
            OCSFXLogger.Log($"[{this}] {bank} (Runtime Bank) Unload - process started at {unloadStartTime} seconds", this, _showDebug);
            RuntimeManager.UnloadBank(bank);
            while (RuntimeManager.HasBankLoaded(bank)) yield return null;
            
            var unloadTime = Time.realtimeSinceStartup - unloadStartTime;
            
            OCSFXLogger.Log($"[{this}] {bank} (Runtime Bank) Unloaded after {unloadTime} seconds", this, _showDebug);
        }

        public void LoadRuntimeBank(string bankName)
        {
            if (!TryGetRuntimeBank(bankName, out var bank))
            {
                OCSFXLogger.LogError($"{name}: {bankName} was not found in RuntimeBanks!", this, _showDebug);
                return;
            }
            
            CoroutineRunner
                .CreateAndRun(Co_LoadRuntimeBank(bank.Name))
                .SetName($"Bank Loader ({bankName})");
        }
        
        private bool TryGetRuntimeBank(string bankName, out EZFMODBank bank)
        {
            bank = null;
            if (_runtimeBanks.Count < 1) return false;
            
            bank = _runtimeBanks.Find(
                bank => bank.Name == bankName);
            if (bank == null) return false;
            
            if (bank) return true;
            
            OCSFXLogger.LogError($"Entry ({bankName}) found, but it has no {nameof(EZFMODBank)} assigned.", this);
            return false;
        }

        public void UnloadRuntimeBank(string bankName)
        {
            if (!TryGetRuntimeBank(bankName, out var bank))
            {
                OCSFXLogger.LogError($"{name}: {bankName} was not found in RuntimeBanks!", this, _showDebug);
                return;
            }
            
            CoroutineRunner
                .CreateAndRun(Co_UnloadRuntimeBank(bank.Name))
                .SetName($"Bank Unloader ({bankName})");
        }

        protected void OnValidate()
        {
            if (_startupBanks == null || _startupBanks.Count < 1)
            {
                _startupBanks ??= new List<EZFMODBank>();
                
                _startupBanks.Add(EZFMODSettings.Get()?.MasterBank);
            }
        }
    }
}