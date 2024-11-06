using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime
{
    public class FallProbe : MonoBehaviour
    {
        public float ProbeDistance = 10f;

        [FormerlySerializedAs("PositiveLayerMask")]
        [Tooltip("Layers that are considered safe ground for a fall.")]
        [FormerlySerializedAs("GroundLayerMask")]
        public LayerMask PositiveLayers;
        [FormerlySerializedAs("NegativeLayerMask")]
        [Tooltip("Layers that are considered dangerous ground for a fall.")]
        [FormerlySerializedAs("KillZoneLayerMask")] public LayerMask NegativeLayers;
        
        [Space]
        public int SafeAlternativePositionProbeCount = 6;
        public float SafeAlternativePositionProbeAngle = 180f;
        public float SafeAlternativePositionProbeRadius = .2f;
        
        private List<Vector3> _alternativeProbePositions = new List<Vector3>();
        
        [Header("Debug")]
        [SerializeField] private bool _showDebug;
        
        public bool IsSafeFall { get; private set; }
        
        private void Update()
        {
            if (!GetProbeResult(transform.position, out var hit)) return;
            
            IsSafeFall = IsSafeProbeResult(hit);
        }
        
        public bool TryGetSafeAlternativePosition(out Vector3 safeAlternativePosition)
        {
            GetAlternativeProbePositions(ref _alternativeProbePositions);
            
            foreach (var probePosition in _alternativeProbePositions)
            {
                if (!GetProbeResult(probePosition, out var hit) || !IsSafeProbeResult(hit)) continue;
                safeAlternativePosition = probePosition;
                return true;
            }

            safeAlternativePosition = Vector3.zero;
            return false;
        }
        
        private void GetAlternativeProbePositions(ref List<Vector3> probePositions)
        {
            probePositions.Clear();
            
            // Trace around in a semicircle with this initial transform as the apex in the forward direction
            // For example, for a 180 degree semicircle, we would trace from -90 to 90 degrees
            // where -90 is leftmost and behind the starting position.
            
            var startingPosition = transform.position - transform.forward * SafeAlternativePositionProbeRadius;
            var halfAngle = SafeAlternativePositionProbeAngle / 2;
            var angleStep = SafeAlternativePositionProbeAngle / SafeAlternativePositionProbeCount;
            
            for (var i = 1; i < SafeAlternativePositionProbeCount; i++)
            {
                var angle = -halfAngle + i * angleStep;
                var direction = Quaternion.Euler(0, angle, 0) * transform.forward;
                var probePosition = startingPosition + direction * SafeAlternativePositionProbeRadius;
                probePositions.Add(probePosition);
            }
        }

        private bool GetProbeResult(Vector3 traceStartPosition, out RaycastHit hit)
        {
            return Physics.Raycast(traceStartPosition, Vector3.down, out hit, ProbeDistance);
        }
        
        private bool IsSafeProbeResult(RaycastHit hit)
        {
            var hitCollider = hit.collider;
            if (!hitCollider) return false;
            
            var hitGameObjectLayer = hitCollider.gameObject.layer;
            
            return PositiveLayers.ContainsLayer(hitGameObjectLayer)
                   && !NegativeLayers.ContainsLayer(hitGameObjectLayer);
        }
        
        private void OnDrawGizmosSelected()
        {
            if (!_showDebug) return;
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, .1f);
            
            Gizmos.color = IsSafeFall ? Color.clear : Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * ProbeDistance);
            
            Gizmos.color = Color.black * .5f;
            GetAlternativeProbePositions(ref _alternativeProbePositions);
            foreach (var probePosition in _alternativeProbePositions)
            {
                Gizmos.DrawLine(probePosition, probePosition + Vector3.down * ProbeDistance);
            }
        }
    }
}