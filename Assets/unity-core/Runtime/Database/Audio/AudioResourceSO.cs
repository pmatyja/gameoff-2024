using System;
using UnityEngine;

public abstract class AudioResourceSO : ResourceSO
{
    [SerializeField]
    [Range(0, 32)]
    public int priority;
    public int Priority => this.priority;
    public abstract AudioLayer Layer { get; }

    [SerializeReference]
    [TypeInstanceSelector]
    public IAudioResource Resource;
    public float Duration => this.Resource?.Duration ?? 0.0f;

    public IAudioSource Create(GameObject anchor = null)
    {
        if (this.Resource == null)
        {
            return null;
        }

        var id = $"AudioSource:{Guid.NewGuid().ToString()}";
        var child = new GameObject(id);

        if (anchor != null)
        {
            child.transform.position = anchor.transform.position;
        }
        else if (GameManager.Instance != null)
        {
            child.transform.position = GameManager.Instance.transform.position;
        }

        return this.Resource.CreateAudioSource(id, this, child);
    }

    public IAudioSource Play(GameObject anchor = null)
    {
        var source = this.Create(anchor);
        source?.Play();
        return source;
    }
}
