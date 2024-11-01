using UnityEngine;

public class NodeGraphPlayer : SceneNodeGraph
{
    [SerializeField]
    private bool autostart;

    protected override void OnEnable()
    {
        base.OnEnable();
        
        if (this.autostart)
        {
            this.graph?.Schedule();
        }
    }
}