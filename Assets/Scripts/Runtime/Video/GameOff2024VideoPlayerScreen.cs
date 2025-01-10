using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(RawImage), typeof(CanvasGroup))]
public class GameOff2024VideoPlayerScreen : OCSFX.Generics.Singleton<GameOff2024VideoPlayerScreen>
{
    private CanvasGroup _canvasGroup;
    
    private static readonly List<GameOff2024VideoPlayer> _registeredVideoPlayers = new List<GameOff2024VideoPlayer>();
    
    [field: Space]
    [SerializeField, Range(0,1)] private float _alpha = 1;

    [RuntimeInitializeOnLoadMethod]
    private static void RuntimeInit()
    {
        Application.quitting += () => _registeredVideoPlayers.Clear();
    }

    protected override void Awake()
    {
        _dontDestroyOnLoad = false;
        base.Awake();
    }

    private void Start()
    {
        if (!_canvasGroup) _canvasGroup = GetComponent<CanvasGroup>();
        Disable();
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
        
        if (!_instance)
        {
            OnInitialized += () => _instance.SetEventBindings(videoPlayer, false);
        }
        else
        {
            _instance.SetEventBindings(videoPlayer, false);
        }
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
        Instance._alpha = 1;
        Instance._canvasGroup.alpha = Instance._alpha;
        Instance._canvasGroup.blocksRaycasts = true;
    }

    private static void Disable()
    {
        Instance._alpha = 0;
        Instance._canvasGroup.alpha = Instance._alpha;
        Instance._canvasGroup.blocksRaycasts = false;
        
        Instance.StopAllCoroutines();
    }
}
