using Unity.Collections;
using UnityEngine;

namespace GameOff2024.SceneLoading
{
    public class SceneLoadDelegate : MonoBehaviour
    {
        [SerializeField, ReadOnly] private SceneLoadManager _sceneLoadManager;

        public void LoadSceneByName(string sceneName)
        {
            SceneLoadManager.LoadScene(sceneName);
        }

        public void ReloadCurrentScene()
        {
            SceneLoadManager.ReloadScene();
        }

        private void Reset()
        {
            ResolveDependencies();
        }

        private void ResolveDependencies()
        {
            if (!_sceneLoadManager) _sceneLoadManager = SceneLoadManager.Get();
        }
    }
}
