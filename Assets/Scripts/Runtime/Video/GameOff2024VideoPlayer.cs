using System;
using OCSFX.Attributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

public class GameOff2024VideoPlayer : Singleton<GameOff2024VideoPlayer>
{
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField, Tooltip("Video clip must be in Assets/StreamingAssets/")] private Object _videoClip;
    [SerializeField, ReadOnly] private string _streamingAssetsPath = "Assets/StreamingAssets/";
    [SerializeField, ReadOnly] private string _relativeVideoPath = string.Empty;
    
    [field: Space]
    [field: SerializeField] public UnityEvent<VideoPlayer> OnVideoStart { get; private set; }
    [field: SerializeField] public UnityEvent<VideoPlayer> OnVideoStop { get; private set; }
    [field: SerializeField] public UnityEvent<VideoPlayer> OnVideoComplete { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        TryUpdateUrl();
    }

    private void OnEnable()
    {
        _videoPlayer.started += OnVideoStart.Invoke;
        _videoPlayer.loopPointReached += OnVideoComplete.Invoke;
    }

    private void OnDisable()
    {
        _videoPlayer.started -= OnVideoStart.Invoke;
        _videoPlayer.loopPointReached -= OnVideoComplete.Invoke;
    }
    
#if UNITY_EDITOR
    [ContextMenu(nameof(EditorPlay))]
    public void EditorPlay() => Play();
    
    [ContextMenu(nameof(EditorStop))]
    public void EditorStop() => Stop();
    
#endif //UNITY_EDITOR

    public static void Play()
    {
        Instance._videoPlayer.Prepare();
        OnPlay();
    }

    public static void Stop()
    {
        OnStop();
    }

    private static void OnPlay()
    {
        Instance._videoPlayer.Play();
    }

    private static void OnStop()
    {   
        Instance._videoPlayer.time = 0;
        Instance._videoPlayer.Stop();
        
        Instance.OnVideoStop.Invoke(Instance._videoPlayer);
    }

    private string GetRelativeVideoPath()
    {
        _streamingAssetsPath = Application.streamingAssetsPath;
        
        var projectRelativeStreamingAssetsPath = _streamingAssetsPath.Replace(Application.dataPath, "Assets") + "/";
        
#if UNITY_EDITOR
        _relativeVideoPath = _videoClip ? AssetDatabase.GetAssetPath(_videoClip) : string.Empty;
#endif // UNITY_EDITOR
        
        if (!_relativeVideoPath.Contains(projectRelativeStreamingAssetsPath))
        {
            throw new Exception($"Video clip must be in {projectRelativeStreamingAssetsPath}");
        }
        
        _relativeVideoPath = _relativeVideoPath.Replace(projectRelativeStreamingAssetsPath, string.Empty);

        return _relativeVideoPath;
    }

    private bool TryUpdateUrl()
    {
        _streamingAssetsPath = Application.streamingAssetsPath;
        
        var videoPlayerURL = $"{_streamingAssetsPath}/{GetRelativeVideoPath()}";
        if (videoPlayerURL == _videoPlayer.url) return false;
        
        _videoPlayer.url = videoPlayerURL;
        
        return true;
    }

    private void OnValidate()
    {
        if (!_videoPlayer && !TryGetComponent(out _videoPlayer))
        {
            _videoPlayer = gameObject.AddComponent<VideoPlayer>();
        }
        
        _videoPlayer.source = VideoSource.Url;

        if (!TryUpdateUrl()) return;
        
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        //
        // var currentScene = SceneManager.GetActiveScene();
        // EditorSceneManager.MarkSceneDirty(currentScene);
#endif //UNITY_EDITOR
    }
}
