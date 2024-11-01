using UnityEngine;

public class SphereAreaTrigger : AreaTrigger
{
    [SerializeField]
    [Range(0.0f, 10000.0f)]
    private float radius = 1.0f;

    protected override bool Overlaps()
    {
        return Physics.CheckSphere(this.transform.position, this.radius, layerMask: this.LayerMask.value, QueryTriggerInteraction.Ignore);
    }

    protected override void DrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, this.radius);
    }
}
