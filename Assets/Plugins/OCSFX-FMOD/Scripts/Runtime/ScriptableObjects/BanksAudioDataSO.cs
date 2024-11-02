using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using OCSFX.FMOD.Types;
using OCSFX.Utility.Debug;

namespace OCSFX.FMOD.AudioData
{
    [CreateAssetMenu(menuName = _CREATE_ASSET_MENU_BASE + "Banks", fileName = nameof(BanksAudioDataSO))]
    public class BanksAudioDataSO : AudioDataSO
    {
        [Space]
        [SerializeField, BankRef] private List<string> _startupBanks = new List<string>()
        {
            "Master"
        };
        
        [SerializeField] [Tooltip("A minimum added delay to help ensure that startup banks have sufficient time to load.")]
        [Range(0f, 10f)] private float _startupBankPostLoadBuffer = 0.1f;
        
        [Space]
        [SerializeField] private List<FMODBank> _runtimeBanks = new List<FMODBank>();

        // Properties
        public List<string> StartupBanks => _startupBanks;
        public List<FMODBank> RuntimeBanks => _runtimeBanks;

        public void LoadStartupBanks()
        {
            CoroutineRunner
                .CreateAndRun(Co_LoadStartupBanks(_startupBankPostLoadBuffer))
                .SetName("Bank Loader (Startup Banks)");
        }

        private IEnumerator Co_LoadStartupBanks(float postLoadBuffer = 0f)
        {
            if (OCSFXAudioStatics.StartupBanksAreLoaded) yield break;
            
            var startupBankLoadStartTime = Time.realtimeSinceStartup;
            
            OCSFXLogger.Log($"[{this}] Load Startup Banks - process started at {startupBankLoadStartTime} seconds", this, _showDebug);
        
            while (!RuntimeManager.HaveMasterBanksLoaded) yield return null;
            while (RuntimeManager.AnySampleDataLoading()) yield return null;
            OCSFXLogger.Log($"[{this}] Master Banks ready.", this, _showDebug);
            
            foreach (var bank in _startupBanks)
            {
                if (string.IsNullOrWhiteSpace(bank)) continue;
                
                RuntimeManager.LoadBank(bank, true);
                
                while (!RuntimeManager.HasBankLoaded(bank) || RuntimeManager.AnySampleDataLoading())
                    yield return null;
                
                OCSFXLogger.Log($"[{this}] {bank} (Startup Bank) Loaded.",this, _showDebug);
            }
            
            yield return new WaitForSeconds(postLoadBuffer);

            var startupLoadFinishTime = Time.realtimeSinceStartup;
            
            OCSFXLogger.Log($"[{this}] Load Startup Banks - process completed at {startupLoadFinishTime} seconds", this, _showDebug);
            
            var startupLoadTime = startupLoadFinishTime - startupBankLoadStartTime;
            
            OCSFXLogger.Log($"[{this}] Startup Banks finished loading after {startupLoadTime} seconds", this, _showDebug);
            OCSFXAudioStatics.StartupBanksLoaded?.Invoke();
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
            if (!_runtimeBanks.TryGetBank(bankName, out var bank))
            {
                if (string.IsNullOrWhiteSpace(bank))
                    OCSFXLogger.LogWarning($"[{this}] Received an empty bank load request.", this, _showDebug);
                
                else
                    OCSFXLogger.LogError($"[{this}] {bankName} was not found in RuntimeBanks!", this, _showDebug);
                
                return;
            }
            
            CoroutineRunner
                .CreateAndRun(Co_LoadRuntimeBank(bank))
                .SetName($"Bank Loader ({bankName})");
        }

        public void UnloadRuntimeBank(string bankName)
        {
            if (!_runtimeBanks.TryGetBank(bankName, out var bank))
            {
                OCSFXLogger.LogError($"{name}: {bankName} was not found in RuntimeBanks!", this, _showDebug);
                return;
            }
            
            CoroutineRunner
                .CreateAndRun(Co_UnloadRuntimeBank(bank))
                .SetName($"Bank Unloader ({bankName})");
        }
    }
}