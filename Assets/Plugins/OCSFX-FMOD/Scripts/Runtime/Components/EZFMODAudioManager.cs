using FMOD.Studio;
using FMODUnity;
using OCSFX.EZFMOD;
using OCSFX.EZFMOD.Attributes;
using OCSFX.EZFMOD.Utility.Generics;
using OCSFX.EZFMOD.ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;
using static OCSFX.EZFMOD.EZFMODRuntimeStatics;

namespace OCSFX.EZFMOD.Components
{
    [AddComponentMenu(CREATE_COMPONENT_MENU_BASE + nameof(EZFMODAudioManager))]
    public class EZFMODAudioManager : SingletonMonoBehaviour<EZFMODAudioManager>
    {
        // Fields
        [Header("Settings")]
        [SerializeField, Expandable] private EZFMODVolumeSettingsAudioDataSO _volumeSettings;

        [Header("Audio Data")]
        [SerializeField, Expandable] protected EZFMODBanksAudioDataSO _banksAudioData;
        [SerializeField, Expandable] protected EZFMODSnapshotsAudioDataSO _snapshotsAudioData;
        [SerializeField, Expandable] protected EZFMODAmbienceAudioDataSO _ambienceAudioData;
        [SerializeField, Expandable] protected EZFMODMusicAudioDataSO _musicAudioData;
        [SerializeField, Expandable] protected EZFMODDialogueAudioDataSO _dialogueAudioData;
        [SerializeField, Expandable] protected EZFMODUiAudioDataSO _uiAudioData;

        [Header("Testing/Debugging")]
        [SerializeField] private GameObject _testGameObject;
        [Space]
        [SerializeField] private EventReference _testEventRef;
        [SerializeField] private bool _playTestEventOnStart;
        
        private EventInstance _testEventInstance;
        private GameObject _listenerObject;

        // [SerializeField] private FMODVoiceLine _fmodVoiceLine;

        protected override void Awake()
        {
            base.Awake();
            
            ValidateListener();
            
            if (!Application.isPlaying) return;
            
            _banksAudioData.LoadStartupBanks();
            _volumeSettings.LoadFromPlayerPrefs();
        }

        private void OnEnable() => SubscribeEvents();
        private void OnDisable() => UnsubscribeEvents();

        public static void StartTestEvent()
        {
            Instance._testEventInstance =  Instance._testEventRef.Play(Instance._testGameObject);
        }

        public static void StopTestEvent()
        {
            if (!_instance || !_instance._testEventInstance.isValid()) return;
            _instance._testEventInstance.Stop();
        }

        private void Start()
        {
            var currentScene = SceneManager.GetActiveScene();
            OnSceneLoaded(currentScene, LoadSceneMode.Single);
        }

        private void OnMasterBanksLoaded()
        {
            if (_playTestEventOnStart && !_testEventRef.IsNull)
            {
                StartTestEvent();   
            }
        }

        protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode sceneLoadMode)
        {
            if (_snapshotsAudioData)
            {
                _snapshotsAudioData.ClearAllSnapshots();   
            }
        }

        protected virtual void OnSceneUnloaded(Scene scene)
        {
        }

        // Helpers
        protected virtual void SubscribeEvents()
        {
            EZFMODRuntimeStatics.OnMasterBanksLoaded += OnMasterBanksLoaded;

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        protected virtual void UnsubscribeEvents()
        {
            EZFMODRuntimeStatics.OnMasterBanksLoaded -= OnMasterBanksLoaded;

            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void ValidateListener()
        {
            var mainCamera = Camera.main;
            if (!_listenerObject)
            {
                _listenerObject = mainCamera ? mainCamera.gameObject : Instance.gameObject;
            }

            if (!_listenerObject.TryGetComponent<AudioListener>(out var unityAudioListener)) return;

            Destroy(unityAudioListener);

            if (!_listenerObject.TryGetComponent<StudioListener>(out _))
            {
                _listenerObject.AddComponent<StudioListener>();
            }        
        }

        protected override void OnDestroy()
        {
            StopTestEvent();
            base.OnDestroy();
        }
        
        private void OnValidate()
        {
            _testGameObject = _testGameObject ? _testGameObject : gameObject;
        }
    }
}