using UnityEngine;

namespace OCSFX.EZFMOD.Utility
{
    [System.Serializable]
    public abstract class ColliderGizmoBase
    {
        public Color FillColor = Color.white;
        public Color WireColor = Color.black; 
        
        private float GetRadius(Collider collider)
        {
            switch (collider)
            {
                case BoxCollider boxCollider:
                    return boxCollider.size.magnitude / 2;
                case SphereCollider sphereCollider:
                    return sphereCollider.radius;
                case CapsuleCollider capsuleCollider:
                    return capsuleCollider.radius;
                default:
                    return 0;
            }
        }

        public enum GizmoDrawComponent
        {
            Fill,
            Wire
        }
    }
}