using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(RawImage), typeof(CanvasGroup))]
public class GameOff2024VideoPlayerScreen : Singleton<GameOff2024VideoPlayerScreen>
{
    private CanvasGroup _canvasGroup;
    
    [field: Space]
    [SerializeField, Range(0,1)] private float _alpha = 1;

    private void Start()
    {
        if (!_canvasGroup) _canvasGroup = GetComponent<CanvasGroup>();
        Disable();
    }

    private void OnEnable()
    {
        GameOff2024VideoPlayer.Instance.OnVideoStart.AddListener(OnVideoStart);
        GameOff2024VideoPlayer.Instance.OnVideoStop.AddListener(OnVideoStop);
        GameOff2024VideoPlayer.Instance.OnVideoComplete.AddListener(OnVideoComplete);
    }
    
    private void OnDisable()
    {
        GameOff2024VideoPlayer.Instance.OnVideoStart.RemoveListener(OnVideoStart);
        GameOff2024VideoPlayer.Instance.OnVideoStop.RemoveListener(OnVideoStop);
        GameOff2024VideoPlayer.Instance.OnVideoComplete.RemoveListener(OnVideoComplete);
    }

    private void OnVideoStart(VideoPlayer videoPlayerComp)
    {
        Enable();
    }
    
    private void OnVideoStop(VideoPlayer videoPlayerComp)
    {
        Disable();
    }

    private void OnVideoComplete(VideoPlayer videoPlayerComp)
    {
        Disable();
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
