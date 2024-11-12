using System;
using UnityEngine;

namespace Lavgine
{
    public class BoundsBuilder : MonoBehaviour
    {
        public Bounds bounds = new Bounds(Vector3.zero, new Vector3(6, 6, 6));

        [Header("Debug")]
        [SerializeField]
        private bool showClippingPreview;

        private void Start()
        {
            this.MakeWall(new Vector3(+1, +0, +0));
            this.MakeWall(new Vector3(-1, +0, +0));
            this.MakeWall(new Vector3(+0, +0, +1));
            this.MakeWall(new Vector3(+0, +0, -1));
            this.MakeWall(new Vector3(+0, -1, +0));
        }

        private void OnDrawGizmosSelected()
        {
            if (this.showClippingPreview)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(this.transform.position + this.bounds.center, this.bounds.extents);
            }
            else
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(this.transform.position + this.bounds.center, this.bounds.extents);
            }
            
            Gizmos.DrawCube
            (
                this.transform.position + this.bounds.center - new Vector3(0.0f, this.bounds.extents.y * 0.5f + 0.1f, 0.0f), 
                new Vector3(this.bounds.extents.x, 0.1f, this.bounds.extents.z)
            );
        }

        private void MakeWall(Vector3 axis)
        {
            var collider = this.gameObject.AddComponent<BoxCollider>();

            collider.center = new Vector3
            (
                ( axis.x * (this.bounds.extents.x * 0.5f) ) + this.bounds.center.x + (axis.x * 0.5f), 
                ( axis.y * (this.bounds.extents.y * 0.5f) ) + this.bounds.center.y + (axis.y * 0.5f), 
                ( axis.z * (this.bounds.extents.z * 0.5f) ) + this.bounds.center.z + (axis.z * 0.5f)
            );

            collider.size = new Vector3
            (
                axis.x + ( ( 1 - MathF.Abs(axis.x) ) * this.bounds.extents.x), 
                axis.y + ( ( 1 - MathF.Abs(axis.y) ) * this.bounds.extents.y), 
                axis.z + ( ( 1 - MathF.Abs(axis.z) ) * this.bounds.extents.z)
            );
        }
    }
}
