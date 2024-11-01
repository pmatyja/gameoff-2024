using UnityEngine;

public class SurfaceMaterial2DBehaviour : MonoBehaviour
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
    private PhysicsMaterial2D surfaceMaterial;
    public PhysicsMaterial2D SurfaceMaterial => this.surfaceMaterial;

    private Rigidbody body;

    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
        var newSurfaceMaterial = default(PhysicsMaterial2D);

        var hit = Physics2D.Raycast(this.transform.position + this.offset, Vector2.down, this.length, layerMask: this.layers.value);
        if (hit.collider != default)
        {
            hit.transform.gameObject.TryGetComponent<Collider2D>(out var collider);

            newSurfaceMaterial = collider?.sharedMaterial;
        }

        if (this.surfaceMaterial == newSurfaceMaterial)
        {
            return;
        }

        this.surfaceMaterial = newSurfaceMaterial;

        if (this.surfaceMaterial != default)
        {
            EventBus.Raise(this, new SurfaceMaterial2DEventParameters
            {
                Source = this.gameObject,
                SurfaceMaterial = newSurfaceMaterial
            });
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(this.transform.position + this.offset, this.transform.position + this.offset + Vector3.down * this.length);
    }
}
