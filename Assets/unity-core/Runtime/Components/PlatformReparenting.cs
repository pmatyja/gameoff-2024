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

        private void Update()
        {
            if (Physics.SphereCast(this.transform.position + this.castOrigin, this.castRadius, Vector3.down, out var groundInfo, this.castLength, this.platformMask, QueryTriggerInteraction.Ignore))
            {
                Transform root = groundInfo.transform;

                while (root)
                {
                    if (root.TryGetComponent<BlockMoverScript>(out var _))
                    {
                        break;
                    }

                    if (root.parent == default)
                    {
                        break;
                    }

                    root = root.parent;
                }

                this.transform.SetParent(root);
            }
            else
            {
                this.transform.SetParent(this.defaultParent);
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
