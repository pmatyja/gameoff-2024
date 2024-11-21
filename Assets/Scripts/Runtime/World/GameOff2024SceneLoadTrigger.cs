using System.Collections;
using OCSFX.Utility.Debug;
using Runtime.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.World
{
    [RequireComponent(typeof(BoxCollider))]
    public class GameOff2024SceneLoadTrigger : MonoBehaviour
    {
        [SerializeField, BuildSceneName] string _targetScene;
        [SerializeField] private bool _setAsActiveScene;
        
        [Header("Debug")]
        [SerializeField] private bool _showDebug;
        
        private BoxCollider _boxCollider;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(GameOff2024Statics.GetPlayerTag())) return;
            
            var alreadyLoaded = SceneManager.GetSceneByName(_targetScene).isLoaded;
            if (alreadyLoaded)
            {
                OCSFXLogger.Log($"[{nameof(GameOff2024SceneLoadTrigger)}] {_targetScene} is already loaded. " +
                                $"Aborting load operation", this, _showDebug);
                return;
            }

            var asyncOp = SceneManager.LoadSceneAsync(_targetScene, LoadSceneMode.Additive);
            if (asyncOp == null) return;
            asyncOp.completed += OnSceneAsyncLoadFinished;
            StartCoroutine(Co_OnSceneAsyncLoading(asyncOp));
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(GameOff2024Statics.GetPlayerTag())) return;
            
            var asyncOp = SceneManager.UnloadSceneAsync(_targetScene);
            if (asyncOp == null) return;
            asyncOp.completed += OnSceneAsyncUnloadFinished;
            StartCoroutine(Co_OnSceneAsyncUnloading(asyncOp));
        }
        
        private IEnumerator Co_OnSceneAsyncLoading(AsyncOperation asyncOp)
        {
            while (!asyncOp.isDone)
            {
                OCSFXLogger.Log($"[{nameof(GameOff2024SceneLoadTrigger)}] Loading progress: {asyncOp.progress}",
                    this, _showDebug);
                yield return null;
            }
        }
        
        private void OnSceneAsyncLoadFinished(AsyncOperation asyncOp)
        {
            asyncOp.completed -= OnSceneAsyncLoadFinished;
            OCSFXLogger.Log($"[{nameof(GameOff2024SceneLoadTrigger)}] " +
                            $"{_targetScene} finished loading async at {asyncOp.progress}.", 
                this, _showDebug);
            
            if (_setAsActiveScene)
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(_targetScene));
            }
        }
        
        private IEnumerator Co_OnSceneAsyncUnloading(AsyncOperation asyncOp)
        {
            while (!asyncOp.isDone)
            {
                Debug.Log($"Unloading progress: {asyncOp.progress}");
                yield return null;
            }
        }
        
        private void OnSceneAsyncUnloadFinished(AsyncOperation asyncOp)
        {
            asyncOp.completed -= OnSceneAsyncUnloadFinished;
            OCSFXLogger.Log($"[{nameof(GameOff2024SceneLoadTrigger)}] " +
                            $"{_targetScene} finished unloading async at {asyncOp.progress}.",
                this, _showDebug);
        }

        private void OnValidate()
        {
            if (!_boxCollider) _boxCollider = GetComponent<BoxCollider>();
            _boxCollider.isTrigger = true;
        }

        private void Reset()
        {
            _boxCollider = GetComponent<BoxCollider>();
            _boxCollider.isTrigger = true;
        }
    }
}