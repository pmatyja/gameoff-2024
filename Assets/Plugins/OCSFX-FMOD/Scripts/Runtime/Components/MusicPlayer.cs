using System.Collections.Generic;
using OCSFX.FMOD.AudioData;
using OCSFX.FMOD.Types;
using OCSFX.Utility.Attributes;
using OCSFX.Utility.Debug;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace OCSFX.FMOD.Components
{
    [AddComponentMenu(OCSFXAudioStatics.CREATE_COMPONENT_MENU_BASE + nameof(MusicPlayer))]
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField, Expandable] protected MusicAudioDataSO _musicAudioData;
        [SerializeField, Expandable] protected VolumeSettingsAudioDataSO _volumeSettings;
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
            OCSFXAudioStatics.StartupBanksLoaded += OnMasterBanksLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            if (_musicAudioData)
            {
                if (_resetParametersOnEnable) _musicAudioData.ResetGlobalParameters();
            }
        }
    
        protected virtual void OnDisable()
        {
            OCSFXAudioStatics.StartupBanksLoaded -= OnMasterBanksLoaded;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            
            if (_musicAudioData)
            {
                if (_resetParametersOnEnable) _musicAudioData.ResetGlobalParameters();
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
            if (!OCSFXAudioStatics.StartupBanksAreLoaded)
            {
                OCSFXAudioStatics.StartupBanksLoaded += () =>
                {
                    InvokeSceneLoadedUnityEvents(scene, mode);
                };
                return;
            }
            
            InvokeSceneLoadedUnityEvents(scene, mode);
        }
        
        protected virtual void OnSceneUnloaded(Scene scene)
        {
            if (!OCSFXAudioStatics.StartupBanksAreLoaded)
            {
                OCSFXAudioStatics.StartupBanksLoaded += () =>
                {
                    InvokeSceneUnloadedUnityEvents(scene);
                };
                return;
            }
            
            InvokeSceneUnloadedUnityEvents(scene);
        }

        private void InvokeSceneLoadedUnityEvents(Scene scene, LoadSceneMode mode)
        {
            bool result = false;
            
            foreach (var entry in _sceneUnityEvents)
            {
                if (scene.name.Contains(entry.SceneName))
                {
                    result = true;
                    entry.OnSceneLoaded?.Invoke();
                }
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
            
            SetMute(_mute);
        }

        public void SetMute(bool mute)
        {
            _mute = mute;

            if (!_volumeSettings) return;
            _volumeSettings.SetMute(_musicVolumeParamName, _mute);
        }
        
        public void MusicEventPlay(string musicEventName)
        {
            if (OCSFXAudioStatics.StartupBanksAreLoaded)
            {
                _musicAudioData.MusicEventPlay(musicEventName);
                return;
            }
            
            OCSFXAudioStatics.StartupBanksLoaded += () =>
            {
                _musicAudioData.MusicEventPlay(musicEventName);
            };
        }
        
        public void MusicEventStop(string musicEventName)
        {
            if (OCSFXAudioStatics.StartupBanksAreLoaded)
            {
                _musicAudioData.MusicEventStop(musicEventName);
                return;
            }
            
            OCSFXAudioStatics.StartupBanksLoaded += () =>
            {
                _musicAudioData.MusicEventStop(musicEventName);
            };
        }

        protected virtual void OnValidate()
        {
            SetMute(_mute);
        }
    }

}
