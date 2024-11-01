using UnityEngine;

public class WaveFunctionCollapseBehaviour : MonoBehaviour
{
    [SerializeField]
    [Range(1, int.MaxValue / 10)]
    private int seed = 1;

    private readonly WaveFunctionCollapse waveFunctionCollapse;

    public WaveFunctionCollapseBehaviour()
    {
        this.waveFunctionCollapse = new WaveFunctionCollapse((ulong)this.seed);
    }

    [ContextMenu("Collapse")]
    public void Collapse()
    {
        //this.waveFunctionCollapse.Collapse<T>(this.grid, )
    }
}
