using UnityEngine;

namespace Runtime
{
    public class FallProbe : MonoBehaviour
    {
        public float ProbeDistance = 10f;
        public LayerMask GroundLayerMask;
        
        [Header("Debug")]
        [SerializeField] private bool _showDebug;
        
        public bool IsFallZone { get; private set; }
        
        private void Update()
        {
            IsFallZone = !Physics.Raycast(
                transform.position, Vector3.down, ProbeDistance, GroundLayerMask);
        }
        
        private void OnDrawGizmosSelected()
        {
            if (!_showDebug) return;
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, .1f);
            
            Gizmos.color = IsFallZone ? Color.red : Color.green;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * ProbeDistance);
        }
    }
}