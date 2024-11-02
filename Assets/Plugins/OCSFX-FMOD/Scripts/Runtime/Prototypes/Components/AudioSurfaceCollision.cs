using System;
using OCSFX.FMOD.Types;
using OCSFX.Utility.Debug;
using UnityEngine;

namespace OCSFX.FMOD.Prototype
{
    public class AudioSurfaceCollision: MonoBehaviour
    {
        [SerializeField] private string _tag = "Player";
        [SerializeField] private Collider _collider;
        [SerializeField] private AudioSurface _surface;
        [SerializeField] private int _priority;

        [Space]
        [SerializeField] private bool _showDebug;

        public AudioSurface Surface => _surface;
        
        public static event Action<AudioSurface, int> EnterSurface;
        public static event Action ExitSurface;
        
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            EnterSurface = null;
            ExitSurface = null;
        }

        private void Awake()
        {
            _collider.isTrigger = true;
            
            if (TryGetComponent<SpriteRenderer>(out var spriteRenderer)) spriteRenderer.enabled = false;
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag(_tag)) return;
            
            //Debug.Log($"Player entered {this} : {_surface}");
            EnterSurface?.Invoke(_surface, _priority);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(_tag)) return;
            
            OCSFXLogger.Log($"{_tag} entered {this} : {_surface}", this, _showDebug);
            EnterSurface?.Invoke(_surface, _priority);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(_tag)) return;
            
            OCSFXLogger.Log($"{_tag} existed {this} : {_surface}", this, _showDebug);
            ExitSurface?.Invoke();
        }

        private void OnValidate()
        {
            if (!_collider) TryGetComponent(out _collider);
            _priority = Mathf.Max(0, _priority);
        }
    }
}