using UnityEngine;

public class SurfaceMaterialBehaviour : MonoBehaviour
{
    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float length;

    [SerializeField]
    private LayerMask layers;

    [SerializeField]
    [Readonly]
    private PhysicsMaterial surfaceMaterial;
    public PhysicsMaterial SurfaceMaterial => this.surfaceMaterial;

    private void FixedUpdate()
    {
        if (Physics.Raycast(this.transform.position + this.offset, Vector3.down, out var hit, this.length, layerMask: this.layers.value))
        {
            hit.transform.gameObject.TryGetComponent<Collider>(out var collider);

            this.surfaceMaterial = collider?.material;
        }
        else
        {
            this.surfaceMaterial = null;
        }

        EventBus.Raise(this, new SurfaceMaterialEventParameters
        {
            Source = this.gameObject,
            SurfaceMaterial = this.surfaceMaterial
        });
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(this.transform.position + this.offset, this.transform.position + this.offset + Vector3.down * this.length);
    }
}
