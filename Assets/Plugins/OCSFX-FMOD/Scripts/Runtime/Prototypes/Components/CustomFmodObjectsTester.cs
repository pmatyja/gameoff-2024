using FMODUnity;
using UnityEngine;

namespace OCSFX.FMOD.Prototype
{
    public class CustomFmodObjectsTester : MonoBehaviour
    {
        [SerializeField] private FmodEvent _fmodEvent;
        [SerializeField] private EventReference _eventRef;

        [ContextMenu(nameof(PlayOneShot))]
        public void PlayOneShot()
        {
            if (_fmodEvent) _fmodEvent.PlayOneShot();
        }
        
        [ContextMenu(nameof(PlayEventRef2D))]
        public void PlayEventRef2D()
        {
            if (!_eventRef.IsNull) _eventRef.Play2D();
        }

        [ContextMenu(nameof(Play))]
        public void Play()
        {
            if (_fmodEvent) _fmodEvent.Play(gameObject);
        }

        [ContextMenu(nameof(Stop))]
        public void Stop()
        {
            if (_fmodEvent) _fmodEvent.Stop(gameObject);
        }
    }
}