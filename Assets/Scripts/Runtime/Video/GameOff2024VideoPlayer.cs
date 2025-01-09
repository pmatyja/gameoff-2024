using System;
using OCSFX.Attributes;
using UnityEngine;
using UnityEngine.Video;
using Object = UnityEngine.Object;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif //UNITY_EDITOR

public class GameOff2024VideoPlayer : Singleton<GameOff2024VideoPlayer>
{
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField, Tooltip("Video clip must be in Assets/StreamingAssets/")] private Object _videoClip;
    [SerializeField, ReadOnly] private string _streamingAssetsPath = "Assets/StreamingAssets/";
    [SerializeField, ReadOnly] private string _relativeVideoPath = string.Empty;

    protected override void Awake()
    {
        base.Awake();

        TryUpdateUrl();
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
