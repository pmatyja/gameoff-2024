using System;
using System.Collections;
using OCSFX.Utility.Attributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Runtime.SceneLoading
{
    public class SceneLoadManager : OCSFX.Generics.Singleton<SceneLoadManager>
{
    [SerializeField, Min(0.1f)] private float _defaultLoadTime = 1.5f;
    [SerializeField] private float _fadeDuration = 2f;

    [SerializeField, Button(nameof(ReloadCurrentScene))]
    private bool _reloadCurrentSceneButton;

    [SerializeField] private string _inspectorLoadSceneName;
    [SerializeField, Button(nameof(InspectorLoadScene))]
    private bool _inspectorLoadScene;

    private Scene _cachedScene;

    [SerializeField] private string _gameSceneName;
    [SerializeField] private string _mainMenuSceneName;

    [Header("Events")]
    [SerializeField] private UnityEvent _onSceneLoaded;

    public static event Action<string, float> LoadSceneQueued;
    public static event Action<float> LoadSceneProgress;
    public static event Action LoadSceneComplete;

    private bool _isLoading;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        // Clear out statics
        LoadSceneQueued = null;
        LoadSceneProgress = null;
        LoadSceneComplete = null;
    }

    private void OnEnable()
    {
        RefreshCachedScene();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnGameSceneEvent(object sender, GameSceneEventParameters info)
    {
        Debug.Log($"[{this}] Received event to load {_gameSceneName}", this);
        
        LoadScene(_gameSceneName);
    }
    
    private void OnMainMenuSceneEvent(object sender, MainMenuSceneEventParameters info)
    {
        Debug.Log($"[{this}] Received event to load {_mainMenuSceneName}", this);
        LoadScene(_mainMenuSceneName);
    }

    public static SceneLoadManager Get()
    {
        if (_instance) return _instance;

        _instance = FindObjectOfType<SceneLoadManager>();

        return _instance;
    }

    private void OnQuitPressed()
    {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    [ContextMenu(nameof(ReloadCurrentScene))]
    public void ReloadCurrentScene()
    {
        if (!Application.isPlaying)
        {
            Debug.LogError($"Cannot {nameof(ReloadCurrentScene)} outside of Play mode.", this);
            return;
        }
        
        var currentScene = SceneManager.GetActiveScene().name;
        LoadSceneWithFade(currentScene, _instance._defaultLoadTime);
    }

    public static void ReloadScene()
    {
        _instance.ReloadCurrentScene();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _onSceneLoaded?.Invoke();
    }

    public static void LoadScene(string sceneName)
    {
        LoadSceneWithFade(sceneName, _instance._fadeDuration);
    }

    private static void LoadSceneWithFade(string sceneName, float fadeDuration, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {
        if (string.IsNullOrWhiteSpace(sceneName)) return;
        if (!Application.isPlaying) return;
        if (_instance._isLoading) return;

        LoadSceneQueued?.Invoke(sceneName, fadeDuration);

        _instance.StartCoroutine(Co_LoadSceneWithFade(sceneName, fadeDuration, loadSceneMode));
        _instance._isLoading = true;
    }

    private static IEnumerator Co_LoadSceneWithFade(string sceneName, float fadeDuration,
        LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {
        RefreshCachedScene();

        var timer = 0f;
        var fakeProgress = 0f;

        while (timer < _instance._defaultLoadTime)
        {
            timer += Time.deltaTime;
            fakeProgress = timer / 20f; // Should figure out something less arbitrary for this...
            LoadSceneProgress?.Invoke(fakeProgress);
            yield return null;
        }

        var asyncOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
        if (asyncOperation != null)
        {
            if (loadSceneMode == LoadSceneMode.Single)
            {
                asyncOperation.completed += UnloadScene;
            }

            while (!asyncOperation.isDone)
            {
                var progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
                progress = Mathf.Max(progress, fakeProgress);
                LoadSceneProgress?.Invoke(progress);
                yield return null;
            }
        }

        _instance._isLoading = false;
        LoadSceneComplete?.Invoke();
    }

    private static void UnloadScene(AsyncOperation asyncOperation)
    {
        RefreshCachedScene();
        _instance.StartCoroutine(Co_UnloadScene(asyncOperation));
    }

    private static IEnumerator Co_UnloadScene(AsyncOperation asyncOperation)
    {
        while (!asyncOperation.isDone) yield return null;

        var currentScene = SceneManager.GetActiveScene();
        
        if (_instance._cachedScene != currentScene)
            SceneManager.UnloadSceneAsync(_instance._cachedScene.name);

        asyncOperation.completed -= UnloadScene;
    }

    private static void UnloadScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }

    public void InspectorLoadScene()
    {
        if (!Application.isPlaying)
        {
            Debug.LogError($"Cannot {nameof(InspectorLoadScene)} outside of Play mode.", this);
            return;
        }
        
        var sceneName = _instance._inspectorLoadSceneName;
        if (string.IsNullOrWhiteSpace(sceneName)) return;

        LoadSceneWithFade(sceneName, _instance._defaultLoadTime);
    }

    private static void RefreshCachedScene()
    {
        _instance._cachedScene = SceneManager.GetActiveScene();
    }

    private void OnValidate()
    {
        _defaultLoadTime = Mathf.Max(_defaultLoadTime, _fadeDuration);
    }
}
}
