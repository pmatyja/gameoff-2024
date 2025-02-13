using OCSFX.EZFMOD.Types;
using OCSFX.EZFMOD.Debug;
using UnityEngine;

namespace OCSFX.EZFMOD.Prototypes.Components
{
    public class AudioSurfaceCollision: MonoBehaviour
    {
        [SerializeField] private string _tag = "Player";
        [SerializeField] private Collider _collider;
        [SerializeField] private EZFMODParameterValue _surfaceParameterValue;
        [SerializeField] private int _priority;

        [Space]
        [SerializeField] private bool _showDebug;

        public EZFMODParameterValue Surface => _surfaceParameterValue;
        public int Priority => _priority;

        private void Awake()
        {
            _collider.isTrigger = true;
            
            if (TryGetComponent<SpriteRenderer>(out var spriteRenderer)) spriteRenderer.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(_tag)) return;
            
            if (!other.TryGetComponent(out AudioSurfaceCollisionHandler audioSurfaceCollisionHandler)) return;

            audioSurfaceCollisionHandler.OnSurfaceEnter(this);
            
            OCSFXLogger.Log($"{_tag} entered {this} : {_surfaceParameterValue.name}", this, _showDebug);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(_tag)) return;
            
            if (!other.TryGetComponent(out AudioSurfaceCollisionHandler audioSurfaceCollisionHandler)) return;

            audioSurfaceCollisionHandler.OnSurfaceExit(this);
            
            OCSFXLogger.Log($"{_tag} existed {this} : {_surfaceParameterValue.name}", this, _showDebug);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!other.collider.CompareTag(_tag)) return;
            
            if (!other.gameObject.TryGetComponent(out AudioSurfaceCollisionHandler audioSurfaceCollisionHandler)) return;

            audioSurfaceCollisionHandler.OnSurfaceEnter(this);
            
            OCSFXLogger.Log($"{_tag} entered {this} : {_surfaceParameterValue.name}", this, _showDebug);
        }

        private void OnCollisionExit(Collision other)
        {
            if (!other.collider.CompareTag(_tag)) return;
            
            if (!other.gameObject.TryGetComponent(out AudioSurfaceCollisionHandler audioSurfaceCollisionHandler)) return;

            audioSurfaceCollisionHandler.OnSurfaceExit(this);
            
            OCSFXLogger.Log($"{_tag} existed {this} : {_surfaceParameterValue.name}", this, _showDebug);
        }

        private void OnValidate()
        {
            if (!_collider)
            {
                if (!TryGetComponent(out _collider))
                {
                    OCSFXLogger.LogWarning("Collider is required.", this);
                    
                    _collider = gameObject.AddComponent<BoxCollider>();
                }
            }
            _priority = Mathf.Max(0, _priority);
        }
    }
}