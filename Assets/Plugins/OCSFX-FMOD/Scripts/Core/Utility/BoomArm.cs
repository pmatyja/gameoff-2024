using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace OCSFX.EZFMOD.Utility
{
    public abstract class BoomArm<T> : MonoBehaviour where T : Behaviour
    {
        [field: SerializeField] public T Source { get; protected set; }
        [field: SerializeField] public Transform Target { get; protected set; }
        
        [field: Range(0,1)]
        [field: Tooltip("How much to favor the target position over the source position.")]
        [field: SerializeField] public float TargetWeight = 1f;
        [field: Tooltip("0 = instant, 1 = smooth")]
        [field: SerializeField, Range(0,1)] public float Easing;

        [SerializeField] protected SourceTargetOffsets Offsets;
        
        [Header("Debug")]
        [SerializeField] protected bool _showDebug;
        [SerializeField] protected DebugSettings _debugSettings;
        
        public void SetSource(T source) => Source = source;
        public void SetTarget(Transform target) => Target = target;
        public void SetTargetWeight(float weight) => TargetWeight = weight;
        public void SetSourceOffset(Vector3 offset) => Offsets.SourceOffset = offset;
        public void SetTargetOffset(Vector3 offset) => Offsets.TargetOffset = offset;
        public Vector3 GetSourceOffset() => Offsets.SourceOffset;
        public Vector3 GetTargetOffset() => Offsets.TargetOffset;

        protected void Start()
        {
            UpdateTransform(false);
        }

        private void LateUpdate() => UpdateTransform();
        
        private void UpdateTransform(bool useInterpolation = true)
        {
            if (!Source || !Target) return;
            
            var newPosition = Vector3.Lerp(GetSourcePosition(), GetTargetPosition(), TargetWeight);
            var newRotation = Source.transform.rotation;
            
            if (!useInterpolation)
            {
                transform.SetPositionAndRotation(newPosition, newRotation);
                return;
            }
            
            var interpValue = 3 * (1.1f - Easing) * Time.deltaTime;
            
            transform.position = Vector3.Lerp(transform.position, newPosition, interpValue);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, interpValue);
        }
        
        public Vector3 GetSourcePosition() => Source ? Source.transform.position + GetOffsetRelativeToSourceDirection() : Vector3.zero;
        public Vector3 GetTargetPosition() => Target ? Target.position + GetOffsetRelativeToTargetDirection() : Vector3.zero;
        
        private Vector3 GetSourceToTargetDirection() => (Target.position - Source.transform.position).normalized;
        
        private Vector3 GetOffsetRelativeToTargetDirection() => GetOffsetRelativeToDirection(Offsets.TargetOffset);
        
        private Vector3 GetOffsetRelativeToSourceDirection() => GetOffsetRelativeToDirection(Offsets.SourceOffset);
        
        private Vector3 GetOffsetRelativeToDirection(Vector3 offset)
        {
            // Given the direction between the source and the target, apply the offset relative to that direction, but keep the y (up) component world-relative.
            var direction = GetSourceToTargetDirection();
            var right = Vector3.Cross(Vector3.up, direction).normalized;
        
            // Calculate the transformed offset
            var transformedOffset = new Vector3(
                offset.x * right.x + offset.z * direction.x,
                offset.y,
                offset.x * right.z + offset.z * direction.z
            );
        
            return transformedOffset;
        }
        
        protected virtual void OnValidate()
        {
            UpdateTransform();
            
#if UNITY_EDITOR
            RegisterEditorUpdate();
#endif //UNITY_EDITOR
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if (!_showDebug) return;

            if (_debugSettings.DrawStep == DebugSettings.GizmoDrawStep.Selected)
            {
                DrawDebugGizmos();
            }
        }
        
        protected virtual void OnDrawGizmos()
        {
            if (!_showDebug) return;

            if (_debugSettings.DrawStep == DebugSettings.GizmoDrawStep.Always)
            {
                DrawDebugGizmos();
            }
        }

        private void DrawDebugGizmos()
        {
            Gizmos.color = _debugSettings.ThisColor;
            Gizmos.DrawWireSphere(transform.position, _debugSettings.DrawSphereRadius);

            if (!Source || !Target) return;
            
            Gizmos.color = _debugSettings.LerpLineColor;
            Gizmos.DrawLine(GetSourcePosition(), GetTargetPosition());
            Gizmos.DrawWireSphere(GetTargetPosition(), .1f);
                
            Gizmos.color = _debugSettings.TargetColor;
            Gizmos.DrawWireSphere(Target.transform.position, _debugSettings.DrawSphereRadius);

            Gizmos.color = _debugSettings.SourceToTargetLineColor * 0.5f;
                
            Gizmos.DrawLine(transform.position, Target.transform.position);
        }
        
#if UNITY_EDITOR
        private bool _isEditorUpdateRegistered;

        private void RegisterEditorUpdate()
        {
            if (_isEditorUpdateRegistered) return;
            
            EditorApplication.update += OnEditorUpdate;
            _isEditorUpdateRegistered = true;
        }
        
        private void UnregisterEditorUpdate()
        {
            if (!_isEditorUpdateRegistered) return;
            
            EditorApplication.update -= OnEditorUpdate;
            _isEditorUpdateRegistered = false;
        }
        
        private void OnEditorUpdate()
        {
            if (Source && Target)
            {
                UpdateTransform(false);
            }

            if (Application.isPlaying)
            {
                UnregisterEditorUpdate();
            }
            else if (EditorApplication.isCompiling)
            {
                RegisterEditorUpdate();
            }
        }
        
#endif //UNITY_EDITOR

        [System.Serializable]
        protected class DebugSettings
        {
            public GizmoDrawStep DrawStep = GizmoDrawStep.Selected;
            public float DrawSphereRadius = 0.5f;
            public Color ThisColor = Color.cyan;
            public Color TargetColor = Color.yellow;
            public Color LerpLineColor = Color.red;
            public Color SourceToTargetLineColor = Color.green;

            public enum GizmoDrawStep
            {
                Selected,
                Always
            }
        }

        [System.Serializable]
        protected class SourceTargetOffsets
        {
            public Vector3 SourceOffset;
            public Vector3 TargetOffset;
        }
    }
}