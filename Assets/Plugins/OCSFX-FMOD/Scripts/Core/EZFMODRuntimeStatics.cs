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
        public static bool MasterBanksLoaded { get; private set; } = false;
        public static Action OnMasterBanksLoaded;

        private const string DEV_NAME = "OCSFX";
        private const string PACKAGE_NAME = "EZFMOD";
        public const string MENU_ITEM_ROOT = DEV_NAME + "/" + PACKAGE_NAME;
        public const string PLUGIN_FOLDER_PATH = "Assets/Plugins/" + MENU_ITEM_ROOT;
        public const string CREATE_COMPONENT_MENU_BASE = MENU_ITEM_ROOT + "/";
        
        public static EventInstance INVALID_EVENT_INSTANCE = default;
        public static PARAMETER_ID INVALID_PARAMETER_ID = default;
        public static GUID INVALID_GUID = default;
        
        private static CoroutineRunner _coroutineRunner;
        private static Coroutine _loadMasterBanksCoroutine;

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
            
            LoadMasterBanks();
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
        
        public static void LoadMasterBanks()
        {
            if (_loadMasterBanksCoroutine != null) return;
            
            _loadMasterBanksCoroutine = RunCoroutine(Co_LoadMasterBanks());
        }
        
        private static IEnumerator Co_LoadMasterBanks()
        {
            if (MasterBanksLoaded) yield break;
            
            var startLoadBanksTime = Time.realtimeSinceStartup;
            OCSFXLogger.Log($"[{nameof(EZFMODRuntimeStatics)}] {nameof(LoadMasterBanks)} : process started at {startLoadBanksTime} seconds");

            var printedWaitingMessage = false;
            
            while (!RuntimeManager.IsInitialized || !RuntimeManager.StudioSystem.isValid())
            {
                if (!printedWaitingMessage)
                {
                    printedWaitingMessage = true;
                    OCSFXLogger.Log($"[{nameof(EZFMODRuntimeStatics)}] {nameof(LoadMasterBanks)} : Waiting for RuntimeManager to initialize...");
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

            if (masterBank && ezfmodSettings.LoadMasterBankOnGameStart)
            {
                RuntimeManager.LoadBank(masterBank.Name, true);
                
                while (!RuntimeManager.HasBankLoaded(masterBank.Name) || RuntimeManager.AnySampleDataLoading())
                {
                    yield return null;
                }
            }
            
            // Now load the master banks as defined in the FMOD settings
            OCSFXLogger.Log($"[{nameof(EZFMODRuntimeStatics)}] Loading Master banks...");
            var fmodSettingsMasterBanks = FMODUnity.Settings.Instance.MasterBanks;
            
            if (fmodSettingsMasterBanks is { Count: > 0 })
            {
                foreach (var bank in fmodSettingsMasterBanks)
                {
                    RuntimeManager.LoadBank(bank, true);
                }
                
                while (!RuntimeManager.HaveMasterBanksLoaded) yield return null;
            }

            if (RuntimeManager.AnySampleDataLoading())
            {
                OCSFXLogger.Log($"[{nameof(EZFMODRuntimeStatics)}] Waiting for all sample data to load...");   
            }
            while (RuntimeManager.AnySampleDataLoading()) yield return null;
            
            OCSFXLogger.Log($"[{nameof(EZFMODRuntimeStatics)}] Master Banks ready.");

            var postLoadBuffer = ezfmodSettings.MasterBanksPostLoadBuffer;
            
            if (postLoadBuffer > 0)
            {
                OCSFXLogger.Log($"[{nameof(EZFMODRuntimeStatics)}] {nameof(LoadMasterBanks)} : Waiting for Post-load buffer of {postLoadBuffer} seconds...");
                yield return new WaitForSeconds(postLoadBuffer);
            }

            var startupLoadFinishTime = Time.realtimeSinceStartup;
            var totalTime = startupLoadFinishTime - startLoadBanksTime;
            
            OCSFXLogger.Log($"[{nameof(EZFMODRuntimeStatics)}] {nameof(LoadMasterBanks)} : Finished after {Math.Round(totalTime, 2)} seconds");
            
            OnMasterBanksLoadComplete();
        }

        private static void OnMasterBanksLoadComplete()
        {
            OnMasterBanksLoaded?.Invoke();
            MasterBanksLoaded = true;
            _loadMasterBanksCoroutine = null;
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
            return GetCoroutineRunner().Run(routine, false);
        }
        
        private static void Shutdown()
        {
            OnMasterBanksLoaded = null;
            MasterBanksLoaded = false;
            
            if (_coroutineRunner)
            {
                Object.Destroy(_coroutineRunner.gameObject);
            }
        }
    }
}

