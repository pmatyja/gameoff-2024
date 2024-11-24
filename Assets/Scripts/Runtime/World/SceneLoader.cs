using System;
using Runtime.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.World
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private LoadSceneAction[] _loadSceneActions;

        public void Load()
        {
            foreach (var loadSceneAction in _loadSceneActions)
            {
                loadSceneAction.LoadScene();
            }
        }

        public void Unload()
        {
            foreach (var loadSceneAction in _loadSceneActions)
            {
                loadSceneAction.UnloadScene();
            }
        }

        [Serializable]
        private class LoadSceneAction
        {
            [SerializeField, BuildSceneName] private string _sceneName;
            [SerializeField] private LoadSceneMode _loadSceneMode;
            [SerializeField] private LoadSceneMethod _loadSceneMethod;
            
            public void LoadScene()
            {
                switch (_loadSceneMethod)
                {
                    case LoadSceneMethod.Blocking:
                        SceneManager.LoadScene(_sceneName, _loadSceneMode);
                        break;
                    default:
                    case LoadSceneMethod.Async:
                        SceneManager.LoadSceneAsync(_sceneName, _loadSceneMode);
                        break;
                }
            }
            
            public void UnloadScene()
            {
                SceneManager.UnloadSceneAsync(_sceneName);
            }
        }
        
        public enum LoadSceneMethod
        {
            Blocking,
            Async
        }
    }
}