using System;
using OCSFX.Utility.Debug;
using Runtime.Utility;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.World
{
    [RequireComponent(typeof(BoxCollider))]
    public class SceneLoaderTriggerSimple : MonoBehaviour
    {
        [BuildSceneName] public string SceneName;
        [TagField] public string PlayerTag = "Player";

        [Header("Debug")]
        [SerializeField] private bool _showDebug;
        
        private BoxCollider _boxCollider;

        private void Awake()
        {
            _boxCollider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            OCSFXLogger.Log($"{nameof(OnTriggerEnter)} : {other.name}", this, _showDebug);
            
            if (other.CompareTag(PlayerTag))
            {
                OCSFXLogger.Log($"{PlayerTag} entered the trigger.", this, _showDebug);
                
                SceneManager.LoadScene(SceneName);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position + _boxCollider.center, _boxCollider.size);
        }

        private void OnValidate()
        {
            if (!_boxCollider) _boxCollider = GetComponent<BoxCollider>();
        }

        private void Reset()
        {
            _boxCollider = GetComponent<BoxCollider>();
            _boxCollider.isTrigger = true;
        }
    }
}