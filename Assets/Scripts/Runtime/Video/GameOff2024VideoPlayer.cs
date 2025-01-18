using System;
using System.Collections;
using OCSFX.Attributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

public class GameOff2024VideoPlayer : MonoBehaviour
{
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField, Tooltip("Video clip must be in Assets/StreamingAssets/")] private Object _videoClip;
    [SerializeField, ReadOnly] private string _streamingAssetsPath = "Assets/StreamingAssets/";
    [SerializeField, ReadOnly] private string _relativeVideoPath = string.Empty;
    [SerializeField, Tooltip("OnVideoComplete will invoke this amount of time (in seconds) before the actual end of the video.")]
    private float _endingBuffer = 1f;
    
    public float EndingBuffer => _endingBuffer;
    
    [field: Space]
    [field: SerializeField] public UnityEvent<GameOff2024VideoPlayer> OnVideoStart { get; private set; }
    [field: SerializeField] public UnityEvent<GameOff2024VideoPlayer> OnVideoStop { get; private set; }
    [field: SerializeField] public UnityEvent<GameOff2024VideoPlayer> OnVideoComplete { get; private set; }
    
    private Coroutine _endingBufferCoroutine;
    private Coroutine _playStatusCoroutine;
    
    [Header("Debug")]
    [SerializeField, ReadOnly] private bool _isPlaying;
    [SerializeField, ReadOnly] private bool _isPaused;
    [SerializeField, ReadOnly] private float _videoLength;
    
    public VideoPlayer VideoPlayer => _videoPlayer;
    
    private void Awake()
    {
        TryUpdateUrl();
        _videoPlayer.Prepare();
        
        _videoPlayer.prepareCompleted += (_) =>
        {
            _videoLength = (float)_videoPlayer.length;
        };
    }

    private void OnEnable()
    {
        _videoPlayer.started += OnVideoPlayerStarted;
        
        GameOff2024VideoPlayerScreen.RegisterVideoPlayer(this);
        
        var inputHandler = InputHandler.Get();
        if (!inputHandler) return;
        
        inputHandler.OnCutsceneSkipInput += OnCutsceneSkipInput;
    }

    private void OnDisable()
    {
        _videoPlayer.started -= OnVideoPlayerStarted;
        
        GameOff2024VideoPlayerScreen.UnregisterVideoPlayer(this);
        
        var inputHandler = InputHandler.Get();
        if (!inputHandler) return;
        
        inputHandler.OnCutsceneSkipInput -= OnCutsceneSkipInput;
    }

    private void OnCutsceneSkipInput()
    {
        if (!_videoPlayer.isPlaying && !_videoPlayer.isPaused) return;
        if (!GameOff2024VideoPlayerScreen.CurrentVideoPlayer) return;
        if (GameOff2024VideoPlayerScreen.CurrentVideoPlayer != this) return;
        
        Stop();
    }

    private void OnVideoPlayerStarted(VideoPlayer videoPlayer)
    {
        OnVideoStart?.Invoke(this);
    }
    
    private void HandleEndingBuffer(VideoPlayer videoPlayer)
    {
        if (_endingBufferCoroutine != null)
        {
            StopCoroutine(_endingBufferCoroutine);
        }
        
        _endingBufferCoroutine = StartCoroutine(Co_HandleEndingBuffer(videoPlayer));
    }

    private IEnumerator Co_HandleEndingBuffer(VideoPlayer videoPlayer)
    {
        _isPlaying = _videoPlayer.isPlaying;
        _isPaused = _videoPlayer.isPaused;
        
        while (_videoPlayer.isPlaying || _videoPlayer.isPaused)
        {
            if (_videoPlayer.time >= _videoPlayer.length - _endingBuffer)
            {
                break;
            }
            yield return null;
        }
        
        _isPlaying = _videoPlayer.isPlaying;
        _isPaused = _videoPlayer.isPaused;
        
        if (_videoPlayer.time < _videoPlayer.length - _endingBuffer)
        {
            // In this case we can assume the video was stopped before it ended.
            OnVideoStop.Invoke(this);
            yield break;
        }
        
        _isPlaying = _videoPlayer.isPlaying;
        _isPaused = _videoPlayer.isPaused;
        
        OnVideoComplete.Invoke(this);
    }

#if UNITY_EDITOR
    [ContextMenu(nameof(EditorPlay))]
    public void EditorPlay() => Play();
    
    [ContextMenu(nameof(EditorStop))]
    public void EditorStop() => Stop();
    
#endif //UNITY_EDITOR

    public void Play()
    {
        TryUpdateUrl();
        if (!_videoPlayer.isPrepared)
        {
            _videoPlayer.Prepare();        
            _videoPlayer.prepareCompleted += (_)=> OnPlay();
        }
        else
        {
            OnPlay();
        }
    }

    public void Stop()
    {
        OnStop();
    }

    private void OnPlay()
    {
        _videoLength = (float)_videoPlayer.length;
        
        _videoPlayer.Play();
        HandleEndingBuffer(_videoPlayer);
    }

    private void OnStop()
    {   
        _videoPlayer.time = 0;
        _videoPlayer.Stop();
        
        OnVideoStop.Invoke(this);
        
        StopCoroutine(_endingBufferCoroutine);
        _endingBufferCoroutine = null;
    }

    private string GetRelativeVideoPath()
    {
        _streamingAssetsPath = Application.streamingAssetsPath;
        
        var projectRelativeStreamingAssetsPath = _streamingAssetsPath.Replace(Application.dataPath, "Assets") + "/";
        
#if UNITY_EDITOR
        _relativeVideoPath = _videoClip ? AssetDatabase.GetAssetPath(_videoClip) : string.Empty;
#endif // UNITY_EDITOR
        
        // if (!_relativeVideoPath.Contains(projectRelativeStreamingAssetsPath))
        // {
        //     throw new Exception($"Video clip must be in {projectRelativeStreamingAssetsPath}");
        // }
        
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
        
        _videoPlayer.source = VideoSource.Url;
    }
}
