using System;
using UnityEngine;

namespace Runtime
{
    public class GroundCheck : MonoBehaviour
    {
        public float Radius = .1f;
        public LayerMask GroundLayerMask;
        
        private readonly Collider[] _groundColliderResults = new Collider[1];

        public bool IsGrounded { get; private set; }
        
        [Header("Debug")]
        [SerializeField] private bool _showDebug;
        
        private void UpdateGroundCheck()
        {
            IsGrounded = Physics.OverlapSphereNonAlloc(transform.position, Radius, _groundColliderResults, GroundLayerMask) > 0;
        }
        
        private void Update()
        {
            UpdateGroundCheck();
        }
        
        private void OnDrawGizmosSelected()
        {
            if (!_showDebug) return;
            
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
    }
}