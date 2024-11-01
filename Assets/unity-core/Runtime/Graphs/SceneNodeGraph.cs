using UnityEngine;

public class SceneNodeGraph : MonoBehaviour, INodeGraphObject
{
    [SerializeReference]
    protected NodeGraph graph;
    public NodeGraph Graph => this.graph;
    
    public Object Handle => this;

    protected virtual void OnEnable()
    {
        this.graph?.OnEnable();
    }

    protected virtual void OnDisable()
    {
        this.graph?.OnDisable();
    }
}