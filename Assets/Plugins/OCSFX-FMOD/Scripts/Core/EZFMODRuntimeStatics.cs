using System;
using System.Collections;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using OCSFX.EZFMOD.Debug;
using OCSFX.EZFMOD.Types;
using OCSFX.EZFMOD.Utility;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OCSFX.EZFMOD
{
    public static class EZFMODRuntimeStatics
    {
        public static bool StartupBanksLoaded { get; private set; } = false;
        public static Action OnStartupBanksLoaded;

        private const string DEV_NAME = "OCSFX";
        private const string PACKAGE_NAME = "EZFMOD";
        public const string MENU_ITEM_ROOT = DEV_NAME + "/" + PACKAGE_NAME;
        public const string PLUGIN_FOLDER_PATH = "Assets/Plugins/" + MENU_ITEM_ROOT;
        public const string CREATE_COMPONENT_MENU_BASE = MENU_ITEM_ROOT + "/";

        public static EventInstance INVALID_EVENT_INSTANCE = default;
        public static EventReference INVALID_EVENT_REFERENCE = default;
        public static PARAMETER_ID INVALID_PARAMETER_ID = default;
        public static GUID INVALID_GUID = default;
        
        private static CoroutineRunner _coroutineRunner;
        private static Coroutine _loadStartupBanksCoroutine;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            Application.quitting += OnApplicationQuit;
            
#if DEVELOPMENT_BUILD
            UnityEngine.Debug.developerConsoleEnabled = true;
            UnityEngine.Debug.unityLogger.logEnabled = true;
            UnityEngine.Debug.developerConsoleVisible = true;
#endif
            
            OCSFXLogger.Log($"[{nameof(EZFMODRuntimeStatics)}] Initialized.");
            
            LoadStartupBanks();
        }

        private static void OnApplicationQuit()
        {
            Application.quitting -= OnApplicationQuit;
            
            Shutdown();
        }

        public static void SetGlobalParameter(string parameterName, float newValue)
        {
            if (!Application.isPlaying) return;
            if (!RuntimeManager.IsInitialized) return;
            
            RuntimeManager.StudioSystem.setParameterByName(parameterName, newValue);
        }
        
        public static float GetGlobalParameterValue(string parameterName)
        {
            if (!Application.isPlaying) return 0;
            if (!RuntimeManager.IsInitialized) return 0;

            RuntimeManager.StudioSystem.getParameterByName(parameterName, out var value);

            return value;
        }
        
        public static void SetGlobalParameter(PARAMETER_ID parameterID, float newValue)
        {
            if (!Application.isPlaying) return;
            if (!RuntimeManager.IsInitialized) return;
            
            RuntimeManager.StudioSystem.setParameterByID(parameterID, newValue);
        }
        
        public static float GetGlobalParameterValue(PARAMETER_ID parameterID)
        {
            if (!Application.isPlaying) return 0;
            if (!RuntimeManager.IsInitialized) return 0;

            RuntimeManager.StudioSystem.getParameterByID(parameterID, out var value);

            return value;
        }
        
        public static EventInstance[] GetEventInstancesFromGUID(GUID guid)
        {
            if (guid == INVALID_GUID) return null;
            if (!RuntimeManager.IsInitialized) return null;
            if (!RuntimeManager.StudioSystem.isValid()) return null;
            
            var eventDescResult = RuntimeManager.StudioSystem.getEventByID(guid, out var eventDescription);
            if (eventDescResult != RESULT.OK)
            {
                OCSFXLogger.LogError($"Failed to get event description from GUID: {guid} | {eventDescResult}");
                return null;
            }
            
            eventDescription.getInstanceList(out var instanceList);
            var returnArray = new EventInstance[instanceList.Length];
            
            for (var i = 0; i < instanceList.Length; i++)
            {
                returnArray[i] = instanceList[i];
            }
            
            return returnArray;
        }
        
        public static EventInstance[] GetEventInstancesFromStudioPath(string studioPath)
        {
            var eventDescResult = RuntimeManager.StudioSystem.getEvent(studioPath, out var eventDescription);
            if (eventDescResult != RESULT.OK)
            {
                OCSFXLogger.LogError($"Failed to get event description from studio path: {studioPath} | {eventDescResult}");
                return null;
            }
            
            eventDescription.getInstanceList(out var instanceList);
            var returnArray = new EventInstance[instanceList.Length];
            
            for (var i = 0; i < instanceList.Length; i++)
            {
                returnArray[i] = instanceList[i];
            }
            
            return returnArray;
        }
        
        public static EventInstance[] GetEventInstancesFromEventReference(EventReference eventReference)
        {
            var eventDesc = RuntimeManager.GetEventDescription(eventReference);
            
            if (!eventDesc.isValid())
            {
                OCSFXLogger.LogError($"Failed to get event description from event reference: {eventReference}");
                return null;
            }
            
            eventDesc.getInstanceList(out var instanceList);
            
            var returnArray = new EventInstance[instanceList.Length];
            for (var i = 0; i < instanceList.Length; i++)
            {
                returnArray[i] = instanceList[i];
            }
            
            return returnArray;
        }
        
        public static bool IsInstanceActive(EventInstance instance)
        {
            if (!instance.isValid()) return false;
            
            instance.getPlaybackState(out var playbackState);

            return playbackState != PLAYBACK_STATE.STOPPED && playbackState != PLAYBACK_STATE.STOPPING;
        }

        public static EZFMODBank GetMasterBank()
        {
            return EZFMODSettings.Get().MasterBank;
        }
        
        public static void LoadStartupBanks()
        {
            // if (_loadStartupBanksCoroutine != null) return;
            
            _loadStartupBanksCoroutine = RunCoroutine(Co_LoadStartupBanks());
        }
        
        private static IEnumerator Co_LoadStartupBanks()
        {
            // if (StartupBanksLoaded) yield break;
            
            var startLoadBanksTime = Time.realtimeSinceStartup;
            OCSFXLogger.Log($"[{nameof(EZFMODRuntimeStatics)}] {nameof(LoadStartupBanks)} : process started at {startLoadBanksTime} seconds");

            var printedWaitingMessage = false;
            
            while (!RuntimeManager.IsInitialized || !RuntimeManager.StudioSystem.isValid())
            {
                if (!printedWaitingMessage)
                {
                    printedWaitingMessage = true;
                    OCSFXLogger.Log($"[{nameof(EZFMODRuntimeStatics)}] {nameof(LoadStartupBanks)} : Waiting for FMOD RuntimeManager to initialize...");
                }
                yield return null;
            }
            
            var ezfmodSettings = EZFMODSettings.Get();
            
            // First off, always load the Master.strings bank because it is required to use StudioPaths
            var masterBank = ezfmodSettings.MasterBank;
            var masterStringBankName = masterBank ? masterBank.Name + ".strings" : "Master.strings";
            
            OCSFXLogger.Log($"[{nameof(EZFMODRuntimeStatics)}] Loading Master strings bank ({masterStringBankName})...");
            RuntimeManager.LoadBank(masterStringBankName);
            while (!RuntimeManager.HasBankLoaded(masterStringBankName))
            {
                yield return null;   
            }
            
            // Now load the master banks as defined in the FMOD settings
            var fmodSettingsMasterBanks = FMODUnity.Settings.Instance.MasterBanks;
            var fmodMasterBanksMessage = fmodSettingsMasterBanks != null ? string.Join(", ", fmodSettingsMasterBanks) : "None";
            
            if (fmodSettingsMasterBanks != null)
            {
                OCSFXLogger.Log($"[{nameof(EZFMODRuntimeStatics)}] Loading FMOD Master banks ({fmodMasterBanksMessage})...");
                
                foreach (var fmodMasterBank in fmodSettingsMasterBanks)
                {
                    RuntimeManager.LoadBank(fmodMasterBank);
                }
                
                foreach (var fmodMasterBank in fmodSettingsMasterBanks)
                {
                    while (!RuntimeManager.HasBankLoaded(fmodMasterBank))
                    {
                        yield return null;
                    }
                }
            }
            
            // Load the EZFMOD Master bank if it exists and the setting is enabled, and if it hasn't been loaded yet
            if (masterBank && ezfmodSettings.LoadMasterBankOnGameStart && !RuntimeManager.HasBankLoaded(masterBank.Name))
            {
                OCSFXLogger.Log($"[{nameof(EZFMODRuntimeStatics)}] Loading {nameof(EZFMOD)} Master bank ({masterBank.Name})...");
                
                RuntimeManager.LoadBank(masterBank.Name);
                
                while (!RuntimeManager.HasBankLoaded(masterBank.Name))
                {
                    yield return null;
                }
            }
            
            // Now load the startup banks as defined in the EZFMOD settings, if they haven't been loaded yet
            var startupBanks = ezfmodSettings.StartupBanks;
            if (startupBanks != null)
            {
                var bankNames = new string[startupBanks.Length];
                for (var i = 0; i < startupBanks.Length; i++)
                {
                    if (!startupBanks[i]) continue;
                    if (RuntimeManager.HasBankLoaded(startupBanks[i].Name)) continue;
                    bankNames[i] = startupBanks[i].Name;
                }

                if (bankNames.Length > 0)
                {
                    var bankNamesMessage = string.Join(", ", bankNames);
                
                    OCSFXLogger.Log($"[{nameof(EZFMODRuntimeStatics)}] Loading {nameof(EZFMOD)} Startup banks ({bankNamesMessage})...");
                
                    foreach (var bank in startupBanks)
                    {
                        if (!bank) continue;
                        if (RuntimeManager.HasBankLoaded(bank.Name)) continue;
                        bank.Load();
                    }

                    foreach (var bankName in bankNames)
                    {
                        if (string.IsNullOrEmpty(bankName)) continue;
                        
                        while (!RuntimeManager.HasBankLoaded(bankName))
                        {
                            yield return null;
                        }
                    }
                }
            }
            
            // If editor, load the Editor-only banks as defined in the EZFMOD settings
            if (Application.isEditor)
            {
                var editorBanks = ezfmodSettings.EditorOnlyBanks;
                if (editorBanks != null)
                {
                    var bankNames = new string[editorBanks.Length];
                    for (var i = 0; i < editorBanks.Length; i++)
                    {
                        if (!editorBanks[i]) continue;
                        if (RuntimeManager.HasBankLoaded(editorBanks[i].Name)) continue;
                        bankNames[i] = editorBanks[i].Name;
                    }

                    if (bankNames.Length > 0)
                    {
                        var bankNamesMessage = string.Join(", ", bankNames);
                
                        OCSFXLogger.Log($"[{nameof(EZFMODRuntimeStatics)}] Loading {nameof(EZFMOD)} Editor banks ({bankNamesMessage})...");
                
                        foreach (var bank in editorBanks)
                        {
                            if (!bank) continue;
                            if (RuntimeManager.HasBankLoaded(bank.Name)) continue;
                            bank.Load();
                        }

                        foreach (var bankName in bankNames)
                        {
                            if (string.IsNullOrEmpty(bankName)) continue;
                        
                            while (!RuntimeManager.HasBankLoaded(bankName))
                            {
                                yield return null;
                            }
                        }
                    }
                }
            }

            if (RuntimeManager.AnySampleDataLoading())
            {
                OCSFXLogger.Log($"[{nameof(EZFMODRuntimeStatics)}] Waiting for all sample data to load...");   
            }
            while (RuntimeManager.AnySampleDataLoading()) yield return null;
            
            OCSFXLogger.Log($"[{nameof(EZFMODRuntimeStatics)}] Startup Banks ready.");

            var postLoadBuffer = ezfmodSettings.StartupBanksPostLoadBuffer;
            
            if (postLoadBuffer > 0)
            {
                OCSFXLogger.Log($"[{nameof(EZFMODRuntimeStatics)}] {nameof(LoadStartupBanks)} : Waiting for Post-load buffer of {postLoadBuffer} seconds...");
                yield return new WaitForSeconds(postLoadBuffer);
            }

            var startupLoadFinishTime = Time.realtimeSinceStartup;
            var totalTime = startupLoadFinishTime - startLoadBanksTime;
            
            OCSFXLogger.Log($"[{nameof(EZFMODRuntimeStatics)}] {nameof(LoadStartupBanks)} : Finished after {Math.Round(totalTime, 2)} seconds");
            
            OnStartupBanksLoadComplete();
        }

        private static void OnStartupBanksLoadComplete()
        {
            OnStartupBanksLoaded?.Invoke();
            StartupBanksLoaded = true;
            _loadStartupBanksCoroutine = null;
        }
        
        private static CoroutineRunner GetCoroutineRunner()
        {
            if (!_coroutineRunner)
            {
                _coroutineRunner =
                    CoroutineRunner.Create($"{nameof(EZFMODRuntimeStatics)} Coroutine Runner")
                        .SetHideFlags(HideFlags.HideAndDontSave)
                        .SetDontDestroyOnLoad();
            }
            
            return _coroutineRunner;
        }
        
        public static Coroutine RunCoroutine(IEnumerator routine)
        {
            if (!Application.isPlaying) return null;
            
            return GetCoroutineRunner().Run(routine, false);
        }
        
        public static void RunOnStartupBanksLoaded(Action action)
        {
            if (!Application.isPlaying) return;
            
            if (StartupBanksLoaded)
            {
                action?.Invoke();
                return;
            }
            
            RunCoroutine(Co_RunOnStartupBanksLoaded(action));
        }
        
        private static IEnumerator Co_RunOnStartupBanksLoaded(Action action)
        {
            yield return _yieldForStartupBanksLoaded;
            
            action?.Invoke();
        }

        private static readonly YieldForStartupBanksLoaded _yieldForStartupBanksLoaded = new YieldForStartupBanksLoaded();
        
        private class YieldForStartupBanksLoaded : CustomYieldInstruction
        {
            public override bool keepWaiting => !StartupBanksLoaded;
        }
        
        // internal static void GetDeferredRuntimeInstance(GUID eventGUID, ref EventInstance instance)
        // {
        //     RunCoroutine(Co_GetDeferredRuntimeInstance(eventGUID, instance => instance = OnDeferredRuntimeInstanceReady(eventGUID)));
        // }
        //
        // private static IEnumerator Co_GetDeferredRuntimeInstance(GUID eventGUID, Action<EventInstance> callback)
        // {
        //     yield return _yieldForStartupBanksLoaded;
        //     
        //     var eventDescResult = RuntimeManager.StudioSystem.getEventByID(eventGUID, out var eventDescription);
        //     if (eventDescResult != RESULT.OK)
        //     {
        //         OCSFXLogger.LogError($"Failed to get event description from GUID: {eventGUID} | {eventDescResult}");
        //         yield break;
        //     }
        //     
        //     eventDescription.createInstance(out var eventInstance);
        //     
        //     callback?.Invoke(eventInstance);
        // }
        //
        // private static EventInstance OnDeferredRuntimeInstanceReady(GUID eventGUID)
        // {
        //     var eventDescResult = RuntimeManager.StudioSystem.getEventByID(eventGUID, out var eventDescription);
        //     if (eventDescResult != RESULT.OK)
        //     {
        //         OCSFXLogger.LogError($"Failed to get event description from GUID: {eventGUID} | {eventDescResult}");
        //         return INVALID_EVENT_INSTANCE;
        //     }
        //     
        //     eventDescription.createInstance(out var eventInstance);
        //
        //     return eventInstance;
        // }
        
        private static void Shutdown()
        {
            OnStartupBanksLoaded = null;
            StartupBanksLoaded = false;
            
            if (_coroutineRunner)
            {
                Object.Destroy(_coroutineRunner.gameObject);
            }
        }
    }
}

