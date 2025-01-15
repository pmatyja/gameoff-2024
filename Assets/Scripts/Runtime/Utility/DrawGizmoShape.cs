using System;
using UnityEngine;

namespace Runtime.Utility
{
    public class DrawGizmoShape : MonoBehaviour
    {
        [SerializeField] private GizmoShape _gizmoShape;
        [SerializeField] private DrawBehavior _drawBehavior;
        [SerializeField] private bool _setPositionManually;
        [SerializeField] private bool _setSizeManually;
    
        [Space]
        [SerializeField] private Vector3 _gizmoPosition;
        [SerializeField] private Color _gizmoColor = Color.white;
        [SerializeField] private float _gizmoSize = 1f;
    
        private void OnDrawGizmos()
        {
            if (_drawBehavior == DrawBehavior.Always)
            {
                DrawGizmo();
            }
        }
    
        private void OnDrawGizmosSelected()
        {
            if (_drawBehavior == DrawBehavior.Selected)
            {
                DrawGizmo();
            }
        }

        private void DrawGizmo()
        {
            if (!_setPositionManually)
            {
                _gizmoPosition = transform.position;
            }
        
            Gizmos.color = _gizmoColor;
        
            switch (_gizmoShape)
            {
                case GizmoShape.Cube:
                    Gizmos.DrawCube(_gizmoPosition, _setSizeManually ? Vector3.one * _gizmoSize : transform.localScale);
                    break;
                case GizmoShape.Sphere:
                    Gizmos.DrawSphere(_gizmoPosition, _setSizeManually ? _gizmoSize : transform.localScale.magnitude * .5776f);
                    break;
                case GizmoShape.WireCube:
                    Gizmos.DrawWireCube(_gizmoPosition, _setSizeManually ? Vector3.one * _gizmoSize : transform.localScale);
                    break;
                case GizmoShape.WireSphere:
                    Gizmos.DrawWireSphere(_gizmoPosition, _setSizeManually ? _gizmoSize : transform.localScale.magnitude * .5776f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private enum GizmoShape
        {
            Cube,
            Sphere,
            WireCube,
            WireSphere
        }
    
        private enum DrawBehavior
        {
            Selected,
            Always
        }
    
        private void OnValidate()
        {
            if (_setPositionManually) return;
        
            _gizmoPosition = transform.position;
        }

        private void Reset()
        {
            _gizmoPosition = transform.position;
        }
    }
}
