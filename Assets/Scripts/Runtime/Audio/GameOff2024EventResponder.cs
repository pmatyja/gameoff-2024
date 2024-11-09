using FMODUnity;
using OCSFX.Utility.Debug;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Audio
{
    public class GameOff2024EventResponder : MonoBehaviour
    {
        [SerializeField] private bool _showDebug;
        
        [SerializeField] private UnityEvent _onCollectableCollected;

        private void OnEnable()
        {
            EventBus.AddListener<CollectableEventParameters>(OnCollectableCollected);
        }

        private void OnDisable()
        {
            EventBus.RemoveListener<CollectableEventParameters>(OnCollectableCollected);
        }

        private void OnCollectableCollected(object sender, CollectableEventParameters info)
        {
            _onCollectableCollected?.Invoke();
            
            OCSFXLogger.Log($"[{nameof(GameOff2024EventResponder)}] {nameof(OnCollectableCollected)}", this, _showDebug);
        }
    }
}