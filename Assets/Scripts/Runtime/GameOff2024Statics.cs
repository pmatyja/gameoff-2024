using System.Collections.Generic;
using OCSFX.FMOD.Components;
using UnityEngine;
using UnityEngine.Rendering;

namespace Runtime
{
    public static class GameOff2024Statics
    {
        private static GameObject _playerGameObject;
        private static Camera _mainCamera;
        private static Volume _globalPostProcessingVolume;
        private static AudioManager _audioManager;
        private static UserInterface _userInterface;
        private static PauseMenuController _pauseMenuController;
        private static UIHoverDetector _uiHoverDetector;
        
        private static readonly Dictionary<float, WaitForSeconds> _waitForSeconds = new Dictionary<float, WaitForSeconds>();
        
        public const string PROJECT_NAME = "GameOff2024";
        public const string MENU_ROOT = PROJECT_NAME + "/";
        
        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            _waitForSeconds.Clear();

            GetMainCamera();
            GetPlayerGameObject();
            GetGlobalPostProcessingVolume();
            GetAudioManager();
            GetUserInterface();
            GetPauseMenuController();
            GetUIHoverDetector();
        }
        
        public static string GetPlayerTag() => GameOff2024GameSettings.Get().PlayerTag;
        
        public static GameObject GetPlayerGameObject()
        {
            if (!_playerGameObject)
            {
                _playerGameObject = GameObject.FindGameObjectWithTag(GameOff2024GameSettings.Get().PlayerTag);
            }
            
            if (!_playerGameObject)
            {
                _playerGameObject = Object.Instantiate(GameOff2024GameSettings.Get().PlayerCharacterPrefab).gameObject;
                _playerGameObject.name = GameOff2024GameSettings.Get().PlayerCharacterPrefab.name;
            }

            return _playerGameObject;
        }
        
        public static Camera GetMainCamera()
        {
            if (!_mainCamera) _mainCamera = Camera.main;

            return _mainCamera;
        }
        
        public static WaitForSeconds GetWaitForSeconds(float seconds)
        {
            if (_waitForSeconds.TryGetValue(seconds, out var waitForSeconds)) return waitForSeconds;
            
            waitForSeconds = new WaitForSeconds(seconds);
            _waitForSeconds.Add(seconds, waitForSeconds);

            return waitForSeconds;
        }
        
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

        public static AudioManager GetAudioManager()
        {
            if (_audioManager) return _audioManager;
            
            // Find the audio manager in the scene
            _audioManager = Object.FindFirstObjectByType<AudioManager>();
            
            // If no audio manager is found, instantiate the one from the game settings
            if (!_audioManager)
            {
                _audioManager = Object.Instantiate(GameOff2024GameSettings.Get().AudioManagerPrefab);
                _audioManager.name = GameOff2024GameSettings.Get().AudioManagerPrefab.name;
            }
            
            return _audioManager;
        }
        
        public static Volume GetGlobalPostProcessingVolume()
        {
            if (_globalPostProcessingVolume) return _globalPostProcessingVolume;
            
            // Find the global post-processing volume in the scene
            var volumesInScene = Object.FindObjectsByType<Volume>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var volume in volumesInScene)
            {
                if (!volume.isGlobal) continue;
                _globalPostProcessingVolume = volume;
                    
                break;
            }

            // If no global post-processing volume is found, instantiate the one from the game settings
            if (!_globalPostProcessingVolume)
            {
                _globalPostProcessingVolume = Object.Instantiate(GameOff2024GameSettings.Get().PostProcessingVolumePrefab);
                _globalPostProcessingVolume.name = GameOff2024GameSettings.Get().PostProcessingVolumePrefab.name;
            }

            return _globalPostProcessingVolume;
        }
        
        public static UserInterface GetUserInterface()
        {
            if (_userInterface) return _userInterface;
            
            // Find the user interface in the scene
            _userInterface = Object.FindFirstObjectByType<UserInterface>();
            
            // If no user interface is found, instantiate the one from the game settings
            if (!_userInterface)
            {
                _userInterface = Object.Instantiate(GameOff2024GameSettings.Get().UserInterfacePrefab);
                _userInterface.name = GameOff2024GameSettings.Get().UserInterfacePrefab.name;
            }
            
            return _userInterface;
        }
        
        public static PauseMenuController GetPauseMenuController()
        {
            if (_pauseMenuController) return _pauseMenuController;
            
            // Find the pause menu controller in the scene
            _pauseMenuController = Object.FindFirstObjectByType<PauseMenuController>();
            
            // If no pause menu controller is found, instantiate the one from the game settings
            if (!_pauseMenuController)
            {
                _pauseMenuController = Object.Instantiate(GameOff2024GameSettings.Get().PauseMenuPrefab);
                _pauseMenuController.name = GameOff2024GameSettings.Get().PauseMenuPrefab.name;
            }
            
            return _pauseMenuController;
        }

        public static UIHoverDetector GetUIHoverDetector()
        {
            if (_uiHoverDetector) return _uiHoverDetector;

            // Find the UI hover detector in the scene
            _uiHoverDetector = Object.FindFirstObjectByType<UIHoverDetector>();

            // If no UI hover detector is found, instantiate the one from the game settings
            if (!_uiHoverDetector)
            {
                _uiHoverDetector = Object.Instantiate(GameOff2024GameSettings.Get().UIHoverDetectorPrefab);
                _uiHoverDetector.name = GameOff2024GameSettings.Get().UIHoverDetectorPrefab.name;
            }

            return _uiHoverDetector;
        }
    }
}