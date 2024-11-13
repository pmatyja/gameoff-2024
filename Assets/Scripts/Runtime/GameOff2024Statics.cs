using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public static class GameOff2024Statics
    {
        private static GameObject _playerGameObject;
        private static Camera _mainCamera;
        private static readonly Dictionary<float, WaitForSeconds> _waitForSeconds = new Dictionary<float, WaitForSeconds>();
        
        public const string PROJECT_NAME = "GameOff2024";
        public const string MENU_ROOT = PROJECT_NAME + "/";
        
        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            _playerGameObject = GameObject.FindGameObjectWithTag(GameOff2024GameSettings.Get().PlayerTag);
            _mainCamera = Camera.main;
            
            _waitForSeconds.Clear();
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
    }
}