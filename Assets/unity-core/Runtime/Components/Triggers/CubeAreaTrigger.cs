using UnityEngine;

public class CubeAreaTrigger : AreaTrigger
{
    [SerializeField]
    private Vector3 extent = Vector3.one;

    protected override bool Overlaps()
    {
        return Physics.CheckBox(this.transform.position, this.extent * 0.5f, this.transform.rotation, layerMask: this.LayerMask.value);
    }

    protected override void DrawGizmos()
    {
        DebugExtension.UsingTransformtion(this.transform, () => Gizmos.DrawWireCube(this.transform.position, this.extent));
    }
}
