using System;
using System.Collections.Generic;
using OCSFX.FMOD.Components;
using Runtime.Collectables;
using Runtime.SceneLoading;
using Runtime.World;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Runtime
{
    public static class GameOff2024Statics
    {
        private static PlayerCharacter _playerCharacter;
        private static Camera _mainCamera;
        private static Volume _globalPostProcessingVolume;
        private static AudioManager _audioManager;
        private static PauseMenuController _pauseMenuController;
        private static UIHoverDetector _uiHoverDetector;
        private static HudController _hudController;
        private static ScreenFade _screenFade;
        private static SceneLoadManager _sceneLoadManager;
        private static LoadingScreen _loadingScreen;
        private static SceneLoader _levelPreloader;
        private static Canvas _uguiCanvas;
        private static ItemInventory _itemInventory;
        
        private static readonly Dictionary<float, WaitForSeconds> _waitForSeconds = new Dictionary<float, WaitForSeconds>();
        
        public const string PROJECT_NAME = "GameOff2024";
        public const string MENU_ROOT = PROJECT_NAME + "/";
        
        public static event Action<int> OnOptionalCollectableTotalChanged;
        
#if UNITY_EDITOR
        [MenuItem(MENU_ROOT + "Initialize Singletons")]
        private static void InitializeSingletons()
        {
            if (Application.isPlaying)
            {
                Debug.LogWarning("This method should only be called in the editor.");
                return;
            }
            
            Initialize();
        }
#endif
        
        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            _waitForSeconds.Clear();
            
            GetAudioManager();

            var canvas = GetUGUICanvas();
            if (Application.isPlaying)
            {
                Object.DontDestroyOnLoad(canvas);
            }
            
            GetScreenFade().transform.SetParent(_uguiCanvas.transform, false);
            GetLoadingScreen().transform.SetParent(_uguiCanvas.transform, false);
            GetSceneLoadManager();
            
            // If the current scene is in the exclusion list, do not initialize the following singletons;
            // They should be initialized only in the main game scenes;
            var activeScene = SceneManager.GetActiveScene();
            if (!GameOff2024GameSettings.Get().ExcludeScenesFromInitialization.Contains(activeScene.name))
            {
                GetGameplayOnlySingletons();
            }

            if (Application.isPlaying)
            {
                Application.quitting += Deinitialize;
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
        }
        
        private static void Deinitialize()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == GameOff2024GameSettings.Get().MainMenuSceneName
                || scene.name == GameOff2024GameSettings.Get().EndingSceneName)
            {
                DestroyGameplayOnlySingletons();
            }
            else
            {
                GetGameplayOnlySingletons();
            }
        }

        private static void DestroyGameplayOnlySingletons()
        {
            TryDestroyGameObject(_playerCharacter);
            TryDestroyGameObject(_mainCamera);
            TryDestroyGameObject(_globalPostProcessingVolume);
            TryDestroyGameObject(_pauseMenuController);
            TryDestroyGameObject(_uiHoverDetector);
            TryDestroyGameObject(_hudController);
            TryDestroyGameObject(_itemInventory);
        }

        private static void GetGameplayOnlySingletons()
        {
            GetMainCamera();
            GetPlayerCharacter();
            GetGlobalPostProcessingVolume();
            GetPauseMenuController();
            GetUIHoverDetector();
            GetHudController();
            GetItemInventory();
        }
        
        private static bool TryDestroyGameObject(Component component)
        {
            if (!component) return false;
            Object.Destroy(component.gameObject);
            return true;
        }
        
        public static ItemInventory GetItemInventory() => 
            GetOrCreateObject(ref _itemInventory, GameOff2024GameSettings.Get().ItemInventoryPrefab);

        public static Canvas GetUGUICanvas() => 
            GetOrCreateObject(ref _uguiCanvas, GameOff2024GameSettings.Get().UGUICanvasPrefab);

        public static ScreenFade GetScreenFade() => 
            GetOrCreateObject(ref _screenFade, GameOff2024GameSettings.Get().ScreenFadePrefab);

        private static void InitializeOnNextSceneLoad(Scene scene, LoadSceneMode mode)
        {
            Initialize();
            SceneManager.sceneLoaded -= InitializeOnNextSceneLoad;
        }

        public static WaitForSeconds GetWaitForSeconds(float seconds)
        {
            if (_waitForSeconds.TryGetValue(seconds, out var waitForSeconds)) return waitForSeconds;
            
            waitForSeconds = new WaitForSeconds(seconds);
            _waitForSeconds.Add(seconds, waitForSeconds);

            return waitForSeconds;
        }
        
        public static string GetPlayerTag() => GameOff2024GameSettings.Get().PlayerTag;
        
        public static Vector3 GetCameraRelativeMoveDirection(Vector2 inputDirection, Transform cameraTransform = null)
        {
            // If there is no camera, use this object's forward vector;
            // Otherwise, use the camera's forward and right vectors to calculate the move direction;
            var forward = cameraTransform ? cameraTransform.forward : GetMainCamera().transform.forward;
            var right = cameraTransform ? cameraTransform.right : GetMainCamera().transform.right;
        
            forward.y = 0f;
            right.y = 0f;
        
            forward.Normalize();
            right.Normalize();
        
            return forward * inputDirection.y + right * inputDirection.x;
        }

        public static PlayerCharacter GetPlayerCharacter() => 
            GetOrCreateObject(ref _playerCharacter, GameOff2024GameSettings.Get().PlayerCharacterPrefab);
        
        public static Camera GetMainCamera()
        {
            if (!_mainCamera) _mainCamera = Camera.main;

            return _mainCamera;
        }

        public static AudioManager GetAudioManager() => 
            GetOrCreateObject(ref _audioManager, GameOff2024GameSettings.Get().AudioManagerPrefab);
        
        public static Volume GetGlobalPostProcessingVolume() => 
            GetOrCreateObjectWithCondition(
                ref _globalPostProcessingVolume, GameOff2024GameSettings.Get().PostProcessingVolumePrefab, 
                volume => volume.isGlobal);
        
        public static PauseMenuController GetPauseMenuController() => 
            GetOrCreateObject(ref _pauseMenuController, GameOff2024GameSettings.Get().PauseMenuPrefab);

        public static UIHoverDetector GetUIHoverDetector() => 
            GetOrCreateObject(ref _uiHoverDetector, GameOff2024GameSettings.Get().UIHoverDetectorPrefab);
        
        public static HudController GetHudController() =>
            GetOrCreateObject(ref _hudController, GameOff2024GameSettings.Get().HudPrefab);
        
        public static LoadingScreen GetLoadingScreen() => 
            GetOrCreateObject(ref _loadingScreen, GameOff2024GameSettings.Get().LoadingScreenPrefab);
        
        public static SceneLoadManager GetSceneLoadManager() =>
            GetOrCreateObject(ref _sceneLoadManager, GameOff2024GameSettings.Get().SceneLoadManagerPrefab);
        
        public static int GetOptionalCollectableTotal() => 
            GameOff2024GameSettings.Get().TotalOptionalCollectables;
        
        // public static SceneLoader GetLevelPreloader() => 
        //     GetOrCreateObject(ref _levelPreloader, GameOff2024GameSettings.Get().LevelPreloaderPrefab);

        public static void RegisterOptionalCollectable(GameOff2024Collectable collectable)
        {
            if (GameOff2024GameSettings.Get().RegisterOptionalCollectable(collectable))
            {
                OnOptionalCollectableTotalChanged?.Invoke(GetOptionalCollectableTotal());
            }
        }


        // HELPER FUNCTIONS
        
        private static T GetOrCreateObject<T> (ref T reference, T prefab) where T : Component
        {
            if (reference) return reference;
            
            reference = Object.FindFirstObjectByType<T>();
            if (reference) return reference;
            
            if (!prefab)
            {
                Debug.LogError($"Prefab of type {typeof(T).Name} is null. Cannot create object.");
                return null;
            }
            
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                reference = PrefabUtility.InstantiatePrefab(prefab) as T;
                if (reference)
                {
                    reference.name = prefab.name;
                    EditorUtility.SetDirty(reference);
                }
                return reference;   
            }
#endif
            reference = Object.Instantiate(prefab);
            reference.name = prefab.name;
            
            return reference;
        }

        private static T GetOrCreateObjectWithCondition<T>(ref T reference, T prefab, System.Func<T, bool> condition)
            where T : Component
        {
            if (reference) return reference;

            foreach (var obj in Object.FindObjectsByType<T>(FindObjectsSortMode.None))
            {
                if (!condition(obj)) continue;
                reference = obj;
                return reference;
            }

            if (!prefab)
            {
                Debug.LogError($"Prefab of type {typeof(T).Name} is null. Cannot create object.");
                return null;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                reference = PrefabUtility.InstantiatePrefab(prefab) as T;
                if (reference)
                {
                    reference.name = prefab.name;
                    EditorUtility.SetDirty(reference);
                }
                return reference;
            }
#endif

            reference = Object.Instantiate(prefab);
            reference.name = prefab.name;
            return reference;
        }
    }
}