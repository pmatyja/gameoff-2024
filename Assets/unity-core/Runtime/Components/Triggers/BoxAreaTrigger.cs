using UnityEngine;

public class BoxAreaTrigger : AreaTrigger
{
    [SerializeField]
    private Vector2 extents;
    
    protected override bool Overlaps()
    {
        return Physics2D.OverlapBox(this.transform.position, this.extents * 0.5f, transform.rotation.eulerAngles.z, layerMask: this.LayerMask.value);
    }

    protected override void DrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        if (this.extents == Vector2.zero)
        {
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
            Gizmos.DrawLine(Vector3.zero, Vector3.forward);
        }
        else
        {
            Gizmos.DrawWireCube(Vector3.zero, this.extents * 0.5f);
        }
    }

    protected virtual void OnValidate()
    {
        this.extents = Vector2.Max(Vector2.zero, this.extents);
    }
}
