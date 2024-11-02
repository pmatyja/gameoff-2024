using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace OCSFX.FMOD
{
    public static class OCSFXAudioStatics
    {
        public static bool StartupBanksAreLoaded { get; private set; } = false;
        public static Action StartupBanksLoaded = OnStartupBanksLoaded;

        internal const string CREATE_COMPONENT_MENU_BASE = "OCSFX/FMOD/";
        internal const string COMPONENT_DISPLAY_NAME_BASE = "OCSFX ";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Initialize()
        {
            StartupBanksLoaded = null;
            StartupBanksAreLoaded = false;
            StartupBanksLoaded += OnStartupBanksLoaded;
        }

        private static void OnStartupBanksLoaded()
        {
            StartupBanksAreLoaded = true;
            StartupBanksLoaded -= OnStartupBanksLoaded;
        }

        public static void SetFMODParameterGlobal(string paramRef, float newValue)
        {
            if (!Application.isPlaying) return;
            if (!RuntimeManager.IsInitialized) return;
            
            RuntimeManager.StudioSystem.setParameterByName(paramRef, newValue);
        }
        
        public static float GetFMODParameterGlobalValue(string paramRef)
        {
            if (!Application.isPlaying) return 0;
            if (!RuntimeManager.IsInitialized) return 0;

            RuntimeManager.StudioSystem.getParameterByName(paramRef, out var value);

            return value;
        }
        
        public static void SetFMODParameterGlobal(PARAMETER_ID parameterID, float newValue)
        {
            if (!Application.isPlaying) return;
            if (!RuntimeManager.IsInitialized) return;
            
            RuntimeManager.StudioSystem.setParameterByID(parameterID, newValue);
        }
        
        public static float GetFMODParameterGlobalValue(PARAMETER_ID parameterID)
        {
            if (!Application.isPlaying) return 0;
            if (!RuntimeManager.IsInitialized) return 0;

            RuntimeManager.StudioSystem.getParameterByID(parameterID, out var value);

            return value;
        }
    }
}

