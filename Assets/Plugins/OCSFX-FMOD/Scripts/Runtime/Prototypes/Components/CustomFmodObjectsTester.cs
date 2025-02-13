using FMODUnity;
using OCSFX.EZFMOD;
using OCSFX.EZFMOD.Types;
using UnityEngine;

namespace OCSFX.EZFMOD.Prototypes.Components
{
    public class CustomFmodObjectsTester : MonoBehaviour
    {
        [SerializeField] private EZFMODEvent _ezfmodEvent;
        [SerializeField] private EventReference _eventRef;

        [ContextMenu(nameof(PlayOneShot))]
        public void PlayOneShot()
        {
            if (_ezfmodEvent) _ezfmodEvent.PlayOneShot();
        }
        
        [ContextMenu(nameof(PlayEventRef2D))]
        public void PlayEventRef2D()
        {
            if (!_eventRef.IsNull) _eventRef.Play2D();
        }

        [ContextMenu(nameof(Play))]
        public void Play()
        {
            if (_ezfmodEvent) _ezfmodEvent.Play(gameObject);
        }

        [ContextMenu(nameof(Stop))]
        public void Stop()
        {
            if (_ezfmodEvent) _ezfmodEvent.Stop(gameObject);
        }
    }
}