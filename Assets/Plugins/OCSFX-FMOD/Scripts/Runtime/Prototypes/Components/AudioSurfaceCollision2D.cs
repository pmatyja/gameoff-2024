using System;
using OCSFX.FMOD.Types;
using UnityEngine;

namespace OCSFX.FMOD.Prototype
{
    public class AudioSurfaceCollision2D: MonoBehaviour
    {
        [SerializeField] private string _tag = "Player";
        [SerializeField] private Collider2D _collider;
        [SerializeField] private AudioSurface _surface;
        [SerializeField] private int _priority;

        public AudioSurface Surface => _surface;
        
        public static event Action<AudioSurface, int> EnterSurface;
        public static event Action ExitSurface;
        
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            EnterSurface = null;
            ExitSurface = null;
        }
        
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (!collision.gameObject.CompareTag(_tag)) return;
            
            EnterSurface?.Invoke(_surface, _priority);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.gameObject.CompareTag(_tag)) return;
            
            EnterSurface?.Invoke(_surface, _priority);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (!collision.gameObject.CompareTag(_tag)) return;

            ExitSurface?.Invoke();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag(_tag)) return;
            
            EnterSurface?.Invoke(_surface, _priority);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(_tag)) return;
            
            EnterSurface?.Invoke(_surface, _priority);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(_tag)) return;
            
            ExitSurface?.Invoke();
        }

        private void OnValidate()
        {
            if (!_collider) TryGetComponent(out _collider);
            _priority = Mathf.Max(0, _priority);
        }
    }
}