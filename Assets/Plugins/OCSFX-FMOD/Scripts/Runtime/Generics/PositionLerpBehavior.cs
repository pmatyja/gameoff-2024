using UnityEngine;

namespace OCSFX.FMOD
{
    public abstract class PositionLerpBehavior<T> : MonoBehaviour where T : Behaviour
    {
        [field: SerializeField] public T Source { get; protected set; }
        [field: SerializeField] public Transform Target { get; protected set; }
        
        [field: Range(0,1)]
        [field: Tooltip("How much the probe should favor the target position over the listener position.")]
        [field: SerializeField] public float TargetWeight = 1f;
        [field: SerializeField] public Vector3 TargetOffset;
        
        [Header("Debug")]
        [SerializeField] protected bool _showDebug;
        [SerializeField] protected DebugSettings _debugSettings;
        
        public void SetTarget(Transform target) => Target = target;
        public void SetTargetWeight(float weight) => TargetWeight = weight;
        public Vector3 GetWorldPosition() => transform.position;
        public void SetTargetOffset(Vector3 offset) => TargetOffset = offset;
        
        public Vector3 GetTargetPosition() => Target ? Target.position + TargetOffset : Vector3.zero;
        
        private void LateUpdate() => UpdateTransform();
        
        private void UpdateTransform()
        {
            var position = Vector3.Lerp(Source.transform.position, GetTargetPosition(), TargetWeight);
            var rotation = Source.transform.rotation;
            transform.SetPositionAndRotation(position, rotation);
        }
        
        protected virtual void OnValidate()
        {
            if (Source && Target)
            {
                UpdateTransform();
            }
        }
        
        protected virtual void OnDrawGizmosSelected()
        {
            if (!_showDebug) return;
            
            Gizmos.color = _debugSettings.ThisColor;
            Gizmos.DrawWireSphere(transform.position, _debugSettings.DrawSphereRadius);

            if (Source && Target)
            {
                Gizmos.color = _debugSettings.LineColor;
                Gizmos.DrawLine(Source.transform.position, GetTargetPosition());
                
                Gizmos.color = _debugSettings.TargetColor;
                Gizmos.DrawWireSphere(GetTargetPosition(), _debugSettings.DrawSphereRadius);
            }
        }
        
        [System.Serializable]
        protected class DebugSettings
        {
            public float DrawSphereRadius = 0.1f;
            public Color ThisColor = Color.cyan;
            public Color TargetColor = Color.yellow;
            public Color LineColor = Color.red;
        }
    }
}