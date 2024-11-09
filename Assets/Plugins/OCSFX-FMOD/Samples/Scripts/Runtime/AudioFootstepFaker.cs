using FMODUnity;
using OCSFX.FMOD.Types;
using UnityEngine;

namespace OCSFX.FMOD
{
    public class AudioFootstepFaker : MonoBehaviour
    {
        [SerializeField] private bool _shouldPlay;
        [SerializeField] private GameObject _sourceObject;

        [SerializeField] [Range(.2f, 2f)] private float _interval = 0.5f;
        private float _footstepIntervalTimer;

        [Header("FMOD")] [SerializeField] private EventReference _footstepEventRef;

        [SerializeField] [ParamRef] private string _surfaceParameter;
        [Space(5)]
        [SerializeField] private AudioSurface _surface;
        
        private Rigidbody _sourceRigidbody;

        public bool ShouldPlay
        {
            get => _shouldPlay;
            set => _shouldPlay = value;
        }

        private void Update()
        {
            if (_sourceObject)
            {
                if (!_sourceRigidbody)
                {
                    if (!_sourceObject.TryGetComponent(out _sourceRigidbody))
                    {
                        return;
                    }
                }
                
                _shouldPlay = _sourceRigidbody.linearVelocity.sqrMagnitude > 0.1f;
            }
            
            if (!_shouldPlay) return;

            if (_footstepIntervalTimer < _interval)
            {
                _footstepIntervalTimer += Time.deltaTime;
            }
            else
            {
                PlayFootstep();
                _footstepIntervalTimer = 0;
            }
        }

        public void SetSurface(AudioSurface surface) => _surface = surface;

        private void PlayFootstep()
        {
            _footstepEventRef.Play(_sourceObject, _surfaceParameter, (int)_surface);
        }

        private void OnValidate()
        {
            if (!_sourceObject) _sourceObject = gameObject;
        }
    }
}
