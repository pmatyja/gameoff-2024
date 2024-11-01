using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public abstract class UiElement
{
    protected MonoBehaviour Behaviour;
    protected VisualElement Root;

    public abstract bool IsVisible(GameMode mode);

    public virtual void Enable(MonoBehaviour behaviour, VisualElement root)
    {
        this.Behaviour = behaviour;
        this.Root = root;
        this.Behaviour.StartCoroutine(this.Animate());
    }

    public virtual void Disable()
    {
    }

    public virtual void Update()
    {
    }

    public virtual IEnumerator Animate()
    {
        yield return null;
    }

    public virtual void OnValidate()
    {
    }
}
