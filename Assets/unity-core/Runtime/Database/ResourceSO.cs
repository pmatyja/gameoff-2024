using UnityEngine;

public abstract class ResourceSO : ScriptableObject, IScriptableObjectAsset
{
    public int Id => this.GetInstanceID();
}
