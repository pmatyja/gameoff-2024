using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage), typeof(CanvasGroup))]
public class GameOff2024VideoPlayerScreen : OCSFX.Generics.Singleton<GameOff2024VideoPlayerScreen>
{
    private CanvasGroup _canvasGroup;
    
    private static readonly List<GameOff2024VideoPlayer> _registeredVideoPlayers = new List<GameOff2024VideoPlayer>();
    
    [field: Space]
    [SerializeField, Range(0,1)] private float _alpha = 1;

    public static event Action OnScreenEnabled;
    public static event Action OnScreenDisabled;

    protected override void Awake()
    {
        _dontDestroyOnLoad = false;
        base.Awake();
        
        if (!_canvasGroup) _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        Disable();
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        
        _registeredVideoPlayers.Clear();
    }

    public static void RegisterVideoPlayer(GameOff2024VideoPlayer videoPlayer)
    {
        if (_registeredVideoPlayers.Contains(videoPlayer)) return;
        
        _registeredVideoPlayers.Add(videoPlayer);

        if (!_instance)
        {
            OnInitialized += () => _instance.SetEventBindings(videoPlayer, true);
        }
        else
        {
            _instance.SetEventBindings(videoPlayer, true);
        }
    }
    
    public static void UnregisterVideoPlayer(GameOff2024VideoPlayer videoPlayer)
    {
        if (!_registeredVideoPlayers.Contains(videoPlayer)) return;
        
        _registeredVideoPlayers.Remove(videoPlayer);

        if (!_instance) return;

        _instance.SetEventBindings(videoPlayer, false);
        
    }

    private void SetEventBindings(GameOff2024VideoPlayer videoPlayer, bool bind)
    {
        if (!videoPlayer)
        {
            Debug.LogError("Video player is null", this);
            return;
        }
        
        if (bind)
        {
            videoPlayer.OnVideoStart.AddListener(OnVideoStart);
            videoPlayer.OnVideoStop.AddListener(OnVideoStop);
            videoPlayer.OnVideoComplete.AddListener(OnVideoComplete);
        }
        else
        {
            videoPlayer.OnVideoStart.RemoveListener(OnVideoStart);
            videoPlayer.OnVideoStop.RemoveListener(OnVideoStop);
            videoPlayer.OnVideoComplete.RemoveListener(OnVideoComplete);   
        }
    }

    private void OnVideoStart(GameOff2024VideoPlayer videoPlayer)
    {
        Enable();
    }
    
    private void OnVideoStop(GameOff2024VideoPlayer videoPlayer)
    {
        Disable();
    }

    private void OnVideoComplete(GameOff2024VideoPlayer videoPlayer)
    {
        if (videoPlayer.VideoPlayer.isPlaying)
        {
            videoPlayer.VideoPlayer.loopPointReached += _ => Disable();
            SceneManager.sceneUnloaded += _ => Disable();
        }
        else
        {
            Disable();
        }
    }

    private static void Enable()
    {
        if (!_instance) return;
        
        _instance._alpha = 1;
        _instance._canvasGroup.alpha = Instance._alpha;
        _instance._canvasGroup.blocksRaycasts = true;
        
        OnScreenEnabled?.Invoke();
    }

    private static void Disable()
    {
        if (!_instance) return;
        
        _instance._alpha = 0;
        _instance._canvasGroup.alpha = Instance._alpha;
        _instance._canvasGroup.blocksRaycasts = false;
        
        OnScreenDisabled?.Invoke();
    }
}
