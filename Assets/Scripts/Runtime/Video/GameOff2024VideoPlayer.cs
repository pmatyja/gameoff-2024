using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace Runtime.Video
{
    [RequireComponent(typeof(VideoPlayer))]
    public class GameOff2024VideoPlayer : MonoBehaviour
    {
        [SerializeField] private VideoPlayer _videoPlayer;
        [field: SerializeField, Readonly] public bool IsPlaying { get; private set; }
        [field: Space]
        [field: SerializeField] public UnityEvent<VideoPlayer> OnVideoStartEvent { get; private set; }
        [field: SerializeField] public UnityEvent<VideoPlayer> OnVideoEndEvent { get; private set; }
        
        private void OnEnable()
        {
            _videoPlayer.started += OnVideoStart;
            _videoPlayer.loopPointReached += OnVideoEnd;
        }

        private void OnDisable()
        {
            _videoPlayer.started -= OnVideoStart;
            _videoPlayer.loopPointReached -= OnVideoEnd;
        }
        
        public void Play()
        {
            _videoPlayer.Play();
        }
        
        // public void Pause()
        // {
        //     _videoPlayer.Pause();
        // }
        
        public void Stop()
        {
            _videoPlayer.Stop();
        }
        
        private void OnVideoStart(VideoPlayer source)
        {
            OnVideoStartEvent?.Invoke(source);
            IsPlaying = true;
        }
        
        private void OnVideoEnd(VideoPlayer source)
        {
            OnVideoEndEvent?.Invoke(source);
            IsPlaying = source.isLooping;
        }
        
        private void OnValidate()
        {
            if (!_videoPlayer) _videoPlayer = GetComponent<VideoPlayer>();
        }
    }
}