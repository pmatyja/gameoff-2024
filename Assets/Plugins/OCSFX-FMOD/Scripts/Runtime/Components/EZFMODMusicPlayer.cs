using System.Collections.Generic;
using OCSFX.EZFMOD.Attributes;
using OCSFX.EZFMOD.Types;
using OCSFX.EZFMOD.Debug;
using OCSFX.EZFMOD.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace OCSFX.EZFMOD.Components
{
    [AddComponentMenu(EZFMODRuntimeStatics.CREATE_COMPONENT_MENU_BASE + nameof(EZFMODMusicPlayer))]
    public class EZFMODMusicPlayer : MonoBehaviour
    {
        [SerializeField, Expandable] protected EZFMODMusicAudioDataSO _musicAudioData;
        [SerializeField, Expandable] protected EZFMODVolumeSettingsAudioDataSO _volumeSettings;
        [SerializeField] protected string _musicVolumeParamName = "Volume_MX";
        
        [Header("Settings")]
        [SerializeField] protected bool _resetParametersOnEnable = true;
        [SerializeField] protected bool _mute = false;

        [Space]
        [SerializeField] protected UnityEvent _onStartupBanksLoaded;
        
        [Space]
        [SerializeField] protected List<SceneUnityEvent> _sceneUnityEvents;
        
        [Header("Debug")]
        [SerializeField] protected bool _showDebug = false;
        
        
        protected virtual void OnEnable()
        {
            EZFMODRuntimeStatics.OnStartupBanksLoaded += OnMasterBanksLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            if (_musicAudioData)
            {
                if (_resetParametersOnEnable) _musicAudioData.ResetParameters();
            }
        }
    
        protected virtual void OnDisable()
        {
            EZFMODRuntimeStatics.OnStartupBanksLoaded -= OnMasterBanksLoaded;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            
            if (_musicAudioData)
            {
                if (_resetParametersOnEnable) _musicAudioData.ResetParameters();
            }
        }

        private void Start()
        {
            var currentScene = SceneManager.GetActiveScene();
            OnSceneLoaded(currentScene, LoadSceneMode.Single);
        }

        // Callbacks
        protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            EZFMODRuntimeStatics.RunOnStartupBanksLoaded(() => InvokeSceneLoadedUnityEvents(scene, mode));
        }
        
        protected virtual void OnSceneUnloaded(Scene scene)
        {
            EZFMODRuntimeStatics.RunOnStartupBanksLoaded(() => InvokeSceneUnloadedUnityEvents(scene));
        }

        private void InvokeSceneLoadedUnityEvents(Scene scene, LoadSceneMode mode)
        {
            var result = false;
            
            foreach (var entry in _sceneUnityEvents)
            {
                if (scene.name != entry.SceneName) continue;
                result = true;
                entry.OnSceneLoaded?.Invoke();
            }
            
            if (!result) OCSFXLogger.LogWarning($"[{this}] {scene.name} was not found in {nameof(_sceneUnityEvents)}", this, _showDebug);
            else OCSFXLogger.Log($"[{this}] {scene.name} was loaded.", this, _showDebug);
        }

        private void InvokeSceneUnloadedUnityEvents(Scene scene)
        {
            bool result = false;
            
            foreach (var entry in _sceneUnityEvents)
            {
                if (scene.name.Contains(entry.SceneName))
                {
                    result = true;
                    entry.OnSceneUnloaded?.Invoke();
                }
            }

            if (!result) OCSFXLogger.LogWarning($"[{name}] {scene.name} was not found in {nameof(_sceneUnityEvents)}", this, _showDebug);
            else OCSFXLogger.Log($"{scene.name} was unloaded.", this, _showDebug);
        }
    
        private void OnMasterBanksLoaded()
        {
            _onStartupBanksLoaded?.Invoke();

            if (_volumeSettings.IsMuted(_musicVolumeParamName) != _mute)
            {
                SetMute(_mute);
            }
        }

        public void SetMute(bool mute)
        {
            _mute = mute;

            if (!_volumeSettings) return;
            _volumeSettings.SetMute(_musicVolumeParamName, _mute);
        }
        
        public void MusicEventPlay(string musicEventName)
        {
            if (!_musicAudioData) return;
            
            EZFMODRuntimeStatics.RunOnStartupBanksLoaded(()=> _musicAudioData.MusicEventPlay(musicEventName));
        }
        
        public void MusicEventStop(string musicEventName)
        {
            if (!_musicAudioData) return;
            
            EZFMODRuntimeStatics.RunOnStartupBanksLoaded(()=> _musicAudioData.MusicEventStop(musicEventName));
        }

        protected virtual void OnValidate()
        {
            SetMute(_mute);
        }
    }

}
