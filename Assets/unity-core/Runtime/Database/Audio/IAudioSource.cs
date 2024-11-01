using UnityEngine;

public abstract class IAudioSource
{
    public string Id { get; }
    public GameObject Anchor { get; }
    public AudioResourceSO Resource { get; }
    public abstract float Volume { get; set; }
    public abstract float Duration { get; }

    protected IAudioSource(string id, GameObject anchor, AudioResourceSO resource)
    {
        this.Id = id;
        this.Anchor = anchor;
        this.Resource = resource;

        AudioSources.Sources.TryAdd(this.Id, this);
    }

    public abstract void Play();
    public abstract void Stop();

    public virtual void Remove()
    {
        GameObject.Destroy(this.Anchor);

        AudioSources.Sources.TryRemove(this.Id, out var _);
    }
}