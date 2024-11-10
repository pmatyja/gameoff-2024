using UnityEngine;

namespace Lavgine
{
    public class BoundsBuilder : MonoBehaviour
    {
        public Bounds bounds = new Bounds(Vector3.zero, new Vector3(6, 6, 6));

        [SerializeField]
        private bool clippingPreview;

        private void Start()
        {
        }

        private void OnDrawGizmos()
        {
            if (this.clippingPreview)
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
    }
}
