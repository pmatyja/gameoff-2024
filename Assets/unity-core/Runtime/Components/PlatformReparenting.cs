using UnityEngine;

namespace Lavgine
{
    public class PlatformReparenting : MonoBehaviour
    {
        [SerializeField]
        private Vector3 castOrigin;

        [SerializeField]
        [Range(0.1f, 2.0f)]
        private float castRadius;
        
        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float castLength;
        
        [SerializeField]
        private LayerMask platformMask;

        [SerializeField]
        private Transform defaultParent;

        private void FixedUpdate()
        {
            if (Physics.SphereCast(transform.position + castOrigin, castRadius, Vector3.down, out var groundInfo, castLength, platformMask, QueryTriggerInteraction.Ignore))
            {
                Transform root = groundInfo.transform;

                while (root && !root.TryGetComponent<BlockMoverScript>(out var _) && root.parent != null)
                {
                    root = root.parent;
                }

                this.transform.SetParent(root);
            }
            else
            {
                this.transform.SetParent(defaultParent);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Debug.DrawLine
            (
                this.transform.position + this.castOrigin, 
                this.transform.position + this.castOrigin + Vector3.down * this.castLength, 
                Color.red
            );

            DebugExtension.DrawWireSphere
            (
                this.transform.position + this.castOrigin + Vector3.down * this.castLength,
                this.castRadius,
                Color.red
            );
        }
    }
}
