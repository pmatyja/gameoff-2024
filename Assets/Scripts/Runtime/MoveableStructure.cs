using OCSFX.Utility.Debug;
using UnityEngine;

namespace Runtime
{
    public class MoveableStructure : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private bool _showDebug;

        private Transform _previousParent;
        private Transform _playerTransform;

        private void Awake()
        {
            _playerTransform = GetPlayerTransform();
        }
        
        private Transform GetPlayerTransform()
        {
            return GameObject.FindGameObjectWithTag(GameOff2024GameSettings.Get().PlayerTag)?.transform;
        }

        private bool IsPlayerOccupying()
        {
            return _playerTransform && _playerTransform.parent == transform;
        }
        
        private void OnPlayerEnter(Collider other)
        {
            OCSFXLogger.Log($"Player entered {name}", this, _showDebug);
            
            if (IsPlayerOccupying()) return;
            
            if (other.CompareTag(GameOff2024GameSettings.Get().PlayerTag))
            {
                _previousParent = other.transform.parent;
                other.transform.SetParent(transform);
            }
        }
        
        private void OnPlayerExit(Collider other)
        {
            OCSFXLogger.Log($"Player exited {name}", this, _showDebug);
            
            if (!IsPlayerOccupying()) return;
            
            if (other.CompareTag(GameOff2024GameSettings.Get().PlayerTag))
            {
                other.transform.SetParent(_previousParent);
            }
        }
    }
}