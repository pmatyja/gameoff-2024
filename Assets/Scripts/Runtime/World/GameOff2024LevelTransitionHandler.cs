using System;
using System.Collections;
using Runtime.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.World
{
    [RequireComponent(typeof(BoxCollider))]
    public class GameOff2024LevelTransitionHandler : MonoBehaviour
    {
        [SerializeField, BuildSceneName] string _targetScene;
        
        private BoxCollider _boxCollider;

        private void OnEnable()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(GameOff2024Statics.GetPlayerTag())) return;

            // var currentScene = SceneManager.GetActiveScene();
            // var currentSkyBox = RenderSettings.skybox;
            
            // var targetScene = SceneManager.GetSceneByName(_targetScene);
            // if (!targetScene.IsValid()) return;

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
                Debug.Log($"Loading progress: {asyncOp.progress}");
                yield return null;
            }
        }
        
        private void OnSceneAsyncLoadFinished(AsyncOperation asyncOp)
        {
            asyncOp.completed -= OnSceneAsyncLoadFinished;
            Debug.Log($"{_targetScene} finished loading async at {asyncOp.progress}.");
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
            Debug.Log($"{_targetScene} finished unloading async at {asyncOp.progress}.");
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