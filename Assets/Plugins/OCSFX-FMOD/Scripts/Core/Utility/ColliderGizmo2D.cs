using System;
using UnityEngine;

namespace OCSFX.EZFMOD.Utility
{
    [System.Serializable]
    public class ColliderGizmo2D : ColliderGizmoBase
    {
        public void Draw(Collider2D collider, GizmoDrawComponent drawComponent)
        {
            if (!collider) return;

            Gizmos.matrix = collider.transform.localToWorldMatrix;

            switch (collider)
            {
                case BoxCollider2D boxCollider:
                    DrawBox(boxCollider.offset, boxCollider.size, drawComponent);
                    break;
                case CircleCollider2D circleCollider:
                    DrawCircle(circleCollider.offset, circleCollider.radius, drawComponent);
                    break;
                case CapsuleCollider2D capsuleCollider:
                    var center = capsuleCollider.offset;
                    var radius = (capsuleCollider.direction == CapsuleDirection2D.Vertical
                        ? capsuleCollider.size.x
                        : capsuleCollider.size.y) / 2;
                    var height = (capsuleCollider.direction == CapsuleDirection2D.Vertical
                        ? capsuleCollider.size.y
                        : capsuleCollider.size.x);
                    if (UseSphereFallback(capsuleCollider))
                    {
                        Gizmos.matrix = Matrix4x4.identity;
                        DrawCircle(collider.transform.position, radius, drawComponent);
                        break;
                    }

                    DrawCapsule(center, capsuleCollider.direction, radius, height, drawComponent);
                    break;
                default:
                    // no-op
                    break;
            }

            // Reset the Gizmos matrix to avoid affecting other Gizmos
            Gizmos.matrix = Matrix4x4.identity;
        }

        private void DrawCapsule(Vector3 center, CapsuleDirection2D direction, float radius, float height,
            GizmoDrawComponent drawComponent)
        {
            switch (drawComponent)
            {
                case GizmoDrawComponent.Fill:
                    Gizmos.color = FillColor;
                    DrawWireCapsule(center, direction, radius, height, FillColor);
                    break;
                case GizmoDrawComponent.Wire:
                    Gizmos.color = WireColor;
                    DrawWireCapsule(center, direction, radius, height, WireColor);
                    break;
            }
        }

        private bool UseSphereFallback(CapsuleCollider2D capsuleCollider)
        {
            return Math.Abs(capsuleCollider.bounds.extents.x - capsuleCollider.bounds.extents.y) < 0.001f;
        }

        private void DrawBox(Vector3 center, Vector3 size, GizmoDrawComponent drawComponent)
        {
            switch (drawComponent)
            {
                case GizmoDrawComponent.Fill:
                    Gizmos.color = FillColor;
                    Gizmos.DrawCube(center, size);
                    break;
                case GizmoDrawComponent.Wire:
                    Gizmos.color = WireColor;
                    Gizmos.DrawWireCube(center, size);
                    break;
            }
        }

        private void DrawCircle(Vector3 center, float radius, GizmoDrawComponent drawComponent)
        {
            switch (drawComponent)
            {
                case GizmoDrawComponent.Fill:
                    Gizmos.color = FillColor;
                    Gizmos.DrawSphere(center, radius);
                    break;
                case GizmoDrawComponent.Wire:
                    Gizmos.color = WireColor;
                    Gizmos.DrawWireSphere(center, radius);
                    break;
            }
        }



        public static void DrawWireCapsule(Vector3 _pos, CapsuleDirection2D direction, float _radius, float _height,
            Color _color = default(Color))
        {
            if (_color != default(Color))
                Gizmos.color = _color;

            var rotation = direction == CapsuleDirection2D.Vertical ? Quaternion.identity : Quaternion.Euler(0, 0, 90);

            var angleMatrix = Matrix4x4.TRS(_pos, rotation, Vector3.one) * Gizmos.matrix;
            Gizmos.matrix = angleMatrix;

            var pointOffset = (_height - (_radius * 2)) / 2;

            // Draw sideways
            Gizmos.DrawWireSphere(Vector3.up * pointOffset, _radius);
            Gizmos.DrawWireSphere(Vector3.down * pointOffset, _radius);
            Gizmos.DrawLine(new Vector3(0, pointOffset, 0), new Vector3(0, -pointOffset, 0));

            // Draw frontways
            Gizmos.DrawLine(new Vector3(-_radius, pointOffset, 0), new Vector3(-_radius, -pointOffset, 0));
            Gizmos.DrawLine(new Vector3(_radius, pointOffset, 0), new Vector3(_radius, -pointOffset, 0));

            // Draw center
            Gizmos.DrawWireSphere(Vector3.up * pointOffset, _radius);
            Gizmos.DrawWireSphere(Vector3.down * pointOffset, _radius);

            // Reset the Gizmos matrix to avoid affecting other Gizmos
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}