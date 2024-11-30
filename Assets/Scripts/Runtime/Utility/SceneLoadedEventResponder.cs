using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Runtime.Utility
{
    public class SceneLoadedEventResponder : MonoBehaviour
    {
        [field: SerializeField] public SceneLoadedEvent[] Events;
        
        private void OnEnable()
        {
            foreach (var sceneLoadedEvent in Events)
            {
                sceneLoadedEvent.SetBinding(true);
            }
        }
        
        private void OnDisable()
        {
            foreach (var sceneLoadedEvent in Events)
            {
                sceneLoadedEvent.SetBinding(false);
            }
        }
        
        [Serializable]
        public class SceneLoadedEvent
        {
            [BuildSceneName] public string SceneName;
            public UnityEvent<Scene, LoadSceneMode> OnSceneLoadedEvent;

            public void SetBinding(bool bind)
            {
                if (bind)
                {
                    SceneManager.sceneLoaded += OnSceneLoaded;
                }
                else
                {
                    SceneManager.sceneLoaded -= OnSceneLoaded;
                }
            }

            private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
            {
                if (scene.name == SceneName)
                {
                    OnSceneLoadedEvent?.Invoke(scene, mode);
                }
            }
        }
    }
}