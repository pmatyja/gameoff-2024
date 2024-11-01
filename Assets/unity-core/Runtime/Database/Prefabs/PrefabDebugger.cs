using UnityEngine;

public class PrefabDebugger : MonoBehaviour
{
    public PrefabSO Prefab;

    private void OnDrawGizmosSelected()
    {
        if (this.Prefab)
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = this.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(this.Prefab.Pivot, this.Prefab.Size);
        }
    }
}
