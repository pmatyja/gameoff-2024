using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Runtime.World
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private LoadSceneAction[] _loadSceneActions;
        
        [field: Header("Events")]
        [field: SerializeField] public UnityEvent OnLoadStart { get; private set; }
        [field: SerializeField] public UnityEvent OnLoadComplete { get; private set; }
        [field: SerializeField] public UnityEvent OnUnloadStart { get; private set; }
        [field: SerializeField] public UnityEvent OnUnloadComplete { get; private set; }

        public void Load()
        {
            var loadAsyncOperations = new List<AsyncOperation>();
            foreach (var loadSceneAction in _loadSceneActions)
            {
                loadSceneAction.OnLoadBegin += asyncOp =>
                {
                    if (asyncOp == null) return;
                    loadAsyncOperations.Add(asyncOp);
                };
                
                loadSceneAction.LoadScene();
            }
            
            OnLoadStart?.Invoke();
            StartCoroutine(Co_HandleAsyncLoadGroup(loadAsyncOperations, OnLoadComplete.Invoke));
        }

        public void Unload()
        {
            var unloadAsyncOperations = new List<AsyncOperation>();
            foreach (var loadSceneAction in _loadSceneActions)
            {
                loadSceneAction.OnUnloadBegin += asyncOp =>
                {
                    if (asyncOp == null) return;
                    unloadAsyncOperations.Add(asyncOp);
                };
                
                loadSceneAction.UnloadScene();
            }
            
            OnUnloadStart?.Invoke();
            StartCoroutine(Co_HandleAsyncLoadGroup(unloadAsyncOperations, OnUnloadComplete.Invoke));
        }
        
        private IEnumerator Co_HandleAsyncLoadGroup(List<AsyncOperation> asyncOperations, Action onComplete = null)
        {
            foreach (var asyncOp in asyncOperations)
            {
                while (!asyncOp.isDone)
                {
                    yield return null;
                }
            }
            
            onComplete?.Invoke();
        }

        [Serializable]
        private class LoadSceneAction
        {
            [SerializeField, BuildSceneName] private string _sceneName;
            [SerializeField] private LoadSceneMode _loadSceneMode;
            [SerializeField] private LoadSceneMethod _loadSceneMethod;
            
            public event Action<AsyncOperation> OnLoadBegin;
            public event Action<AsyncOperation> OnLoadComplete;
            public event Action<AsyncOperation> OnUnloadBegin;
            public event Action<AsyncOperation> OnUnloadComplete;
            
            public void LoadScene()
            {
                if (_loadSceneMode == LoadSceneMode.Additive)
                {
                    var alreadyLoaded = SceneManager.GetSceneByName(_sceneName).isLoaded;
                    if (alreadyLoaded)
                    {
                        Debug.LogWarning($"[{nameof(SceneLoader)}] {_sceneName} is already loaded. " +
                                        $"Aborting load operation");
                        return;
                    }
                }
                
                AsyncOperation asyncOp = null;
                switch (_loadSceneMethod)
                {
                    case LoadSceneMethod.Blocking:
                        SceneManager.LoadScene(_sceneName, _loadSceneMode);
                        break;
                    default:
                    case LoadSceneMethod.Async:
                        asyncOp = SceneManager.LoadSceneAsync(_sceneName, _loadSceneMode);
                        break;
                }
                
                OnLoadBegin?.Invoke(asyncOp);
                if (asyncOp == null) return;
                
                asyncOp.completed += operation =>
                {
                    OnLoadComplete?.Invoke(operation);
                };
            }
            
            public void UnloadScene()
            {
                var asyncOp =SceneManager.UnloadSceneAsync(_sceneName);
                
                OnUnloadBegin?.Invoke(asyncOp);
                if (asyncOp == null) return;
                
                asyncOp.completed += operation =>
                {
                    OnUnloadComplete?.Invoke(operation);
                };
            }
        }
        
        public enum LoadSceneMethod
        {
            Blocking,
            Async
        }
    }
}