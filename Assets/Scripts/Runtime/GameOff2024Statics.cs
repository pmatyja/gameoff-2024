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
        }
        
        public static string GetPlayerTag() => GameOff2024GameSettings.Get().PlayerTag;
        
        public static GameObject GetPlayerGameObject()
        {
            if (!_playerGameObject)
            {
                _playerGameObject = GameObject.FindGameObjectWithTag(GameOff2024GameSettings.Get().PlayerTag);
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
            }

            return _globalPostProcessingVolume;
        }
    }
}