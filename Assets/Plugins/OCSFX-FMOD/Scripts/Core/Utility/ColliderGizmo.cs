using System;
using UnityEngine;

namespace OCSFX.EZFMOD.Utility
{
    [Serializable]
    public class ColliderGizmo : ColliderGizmoBase
    {

        public void Draw(Collider collider, GizmoDrawComponent drawComponent)
        {
            if (!collider) return;
            
            Gizmos.matrix = collider.transform.localToWorldMatrix;

            switch (collider)
            {
                case BoxCollider boxCollider:
                    DrawBox(boxCollider, drawComponent);
                    break;
                case SphereCollider sphereCollider:
                    DrawSphere(sphereCollider, drawComponent);
                    break;
                case CapsuleCollider capsuleCollider:
                    DrawCapsule(capsuleCollider, drawComponent);
                    break;
                default:
                    // no-op
                    break;
            }
        }

        private void DrawCapsule(CapsuleCollider capsuleCollider, GizmoDrawComponent drawComponent)
        {
            var height = capsuleCollider.height;
            var radius = capsuleCollider.radius;
            var direction = capsuleCollider.direction;

            var center = capsuleCollider.center;
            var point1 = Vector3.zero;
            var point2 = Vector3.zero;

            switch (direction)
            {
                case 0: // X-axis
                    point1 = new Vector3(height / 2 - radius, 0, 0);
                    point2 = new Vector3(-height / 2 + radius, 0, 0);
                    break;
                case 1: // Y-axis
                    point1 = new Vector3(0, height / 2 - radius, 0);
                    point2 = new Vector3(0, -height / 2 + radius, 0);
                    break;
                case 2: // Z-axis
                    point1 = new Vector3(0, 0, height / 2 - radius);
                    point2 = new Vector3(0, 0, -height / 2 + radius);
                    break;
            }

            switch (drawComponent)
            {
                case GizmoDrawComponent.Fill:
                    Gizmos.color = FillColor;
                    Gizmos.DrawSphere(center + point1, radius);
                    Gizmos.DrawSphere(center + point2, radius);
                    Gizmos.DrawMesh(CreateCylinderMesh(radius, height - 2 * radius, direction), center);
                    break;
                case GizmoDrawComponent.Wire:
                    Gizmos.color = WireColor;
                    Gizmos.DrawWireSphere(center + point1, radius);
                    Gizmos.DrawWireSphere(center + point2, radius);
                    Gizmos.DrawWireMesh(CreateCylinderMesh(radius, height - 2 * radius, direction), center);
                    break;
            }
        }

        private Mesh CreateCylinderMesh(float radius, float height, int direction)
        {
            var mesh = new Mesh();

            const int segments = 8; // Number of segments for the cylinder
            const int vertexCount = (segments + 1) * 2;
            const int quadCount = segments;

            var vertices = new Vector3[vertexCount];
            var quads = new int[quadCount * 4];
            var normals = new Vector3[vertexCount];

            const float angleStep = 360.0f / segments;
            var vertIndex = 0;
            var quadIndex = 0;

            // Create vertices and normals
            for (var i = 0; i <= segments; i++)
            {
                var angle = Mathf.Deg2Rad * i * angleStep;
                var x = Mathf.Cos(angle) * radius;
                var z = Mathf.Sin(angle) * radius;

                vertices[vertIndex] = new Vector3(x, height / 2, z);
                vertices[vertIndex + segments + 1] = new Vector3(x, -height / 2, z);

                normals[vertIndex] = new Vector3(x, 0, z).normalized;
                normals[vertIndex + segments + 1] = new Vector3(x, 0, z).normalized;

                vertIndex++;
            }

            // Create quads
            for (var i = 0; i < segments; i++)
            {
                var next = (i + 1) % (segments + 1);

                // Side quads
                quads[quadIndex] = i;
                quads[quadIndex + 1] = next;
                quads[quadIndex + 2] = next + segments + 1;
                quads[quadIndex + 3] = i + segments + 1;

                quadIndex += 4;
            }

            mesh.vertices = vertices;
            mesh.SetIndices(quads, MeshTopology.Quads, 0);
            mesh.normals = normals;

            // Adjust the mesh orientation based on the direction
            if (direction == 0) // X-axis
            {
                mesh = RotateMesh(mesh, Quaternion.Euler(0, 0, 90));
            }
            else if (direction == 2) // Z-axis
            {
                mesh = RotateMesh(mesh, Quaternion.Euler(90, 0, 0));
            }

            return mesh;
        }

        private Mesh RotateMesh(Mesh mesh, Quaternion rotation)
        {
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = rotation * vertices[i];
                normals[i] = rotation * normals[i];
            }

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.RecalculateBounds();

            return mesh;
        }

        private void DrawSphere(SphereCollider sphereCollider, GizmoDrawComponent drawComponent)
        {
            var center = sphereCollider.center;
            
            switch (drawComponent)
            {
                case GizmoDrawComponent.Fill:
                    Gizmos.color = FillColor;
                    Gizmos.DrawSphere(center, sphereCollider.radius);
                    break;
                case GizmoDrawComponent.Wire:
                    Gizmos.color = WireColor;
                    Gizmos.DrawWireSphere(center, sphereCollider.radius);
                    break;
            }
        }

        private void DrawBox(BoxCollider boxCollider, GizmoDrawComponent drawComponent)
        {
            var center = boxCollider.center;
            
            switch (drawComponent)
            {
                case GizmoDrawComponent.Fill:
                    Gizmos.color = FillColor;
                    Gizmos.DrawCube(center, boxCollider.size);
                    break;
                case GizmoDrawComponent.Wire:
                    Gizmos.color = WireColor;
                    Gizmos.DrawWireCube(center, boxCollider.size);
                    break;
            }
        }

        public void Draw(Collider[] colliders, GizmoDrawComponent drawComponent)
        {
            foreach (var collider in colliders)
            {
                Draw(collider, drawComponent);
            }
        }
    }
}