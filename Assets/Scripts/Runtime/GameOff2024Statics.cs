using System.Collections.Generic;
using OCSFX.FMOD.Components;
using UnityEngine;
using UnityEngine.Rendering;

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
        private static UserInterface _userInterface;
        private static PauseMenuController _pauseMenuController;
        private static UIHoverDetector _uiHoverDetector;
        
        private static readonly Dictionary<float, WaitForSeconds> _waitForSeconds = new Dictionary<float, WaitForSeconds>();
        
        public const string PROJECT_NAME = "GameOff2024";
        public const string MENU_ROOT = PROJECT_NAME + "/";
        
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

            GetMainCamera();
            GetPlayerCharacter();
            GetGlobalPostProcessingVolume();
            GetAudioManager();
            GetPauseMenuController();
            GetUIHoverDetector();
            GetUserInterface();
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
        
        public static UserInterface GetUserInterface() => 
            GetOrCreateObject(ref _userInterface, GameOff2024GameSettings.Get().UserInterfacePrefab);
        
        public static PauseMenuController GetPauseMenuController() => 
            GetOrCreateObject(ref _pauseMenuController, GameOff2024GameSettings.Get().PauseMenuPrefab);

        public static UIHoverDetector GetUIHoverDetector() => 
            GetOrCreateObject(ref _uiHoverDetector, GameOff2024GameSettings.Get().UIHoverDetectorPrefab);
        
        private static T GetOrCreateObject<T> (ref T reference, T prefab) where T : MonoBehaviour
        {
            if (reference) return reference;
            
            reference = Object.FindFirstObjectByType<T>();
            if (reference) return reference;
            
#if UNITY_EDITOR
            reference = PrefabUtility.InstantiatePrefab(prefab) as T;
            if (reference)
            {
                reference.name = prefab.name;
                EditorUtility.SetDirty(reference);
            }
            return reference;
#endif
            reference = Object.Instantiate(prefab);
            reference.name = prefab.name;
            
            return reference;
        }

        private static T GetOrCreateObjectWithCondition<T>(ref T reference, T prefab, System.Func<T, bool> condition)
            where T : MonoBehaviour
        {
            if (reference) return reference;
            
            var allOfType = Object.FindObjectsByType<T>(FindObjectsSortMode.None);
            
            if (allOfType.Length == 0)
            {
#if UNITY_EDITOR
                reference = PrefabUtility.InstantiatePrefab(prefab) as T;
                if (reference)
                {
                    reference.name = prefab.name;
                    EditorUtility.SetDirty(reference);
                }
                return reference;
#endif
                reference = Object.Instantiate(prefab);
                reference.name = prefab.name;
                return reference;
            }
            
            foreach (var obj in allOfType)
            {
                if (!condition(obj)) continue;
                reference = obj;
                break;
            }

            return reference;
        }
    }
}