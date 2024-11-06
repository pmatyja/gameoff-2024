using System.Collections.Generic;
using UnityEngine;
using Camera = UnityEngine.Camera;

namespace GameOff2024
{
    public static class GameOff2024Statics
    {
        private static GameObject _playerGameObject;
        private static Camera _mainCamera;
        private static readonly Dictionary<float, WaitForSeconds> _waitForSeconds = new Dictionary<float, WaitForSeconds>();
        
        public const string MENU_NAME_BASE = "GameOff2024/";
        
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
    }
}