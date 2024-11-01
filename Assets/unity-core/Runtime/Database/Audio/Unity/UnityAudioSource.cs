using System;
using UnityEngine;

[Serializable]
public class UnityAudioSource : IAudioSource
{
    public override float Volume
    {
        get => this.source?.volume ?? 0.0f;
        set { if (this.source != null) { this.source.volume = Mathf.Clamp01(value); } }
    }
    public override float Duration { get; }

    [SerializeField]
    [Readonly]
    private UnityEngine.AudioSource source;

    public UnityAudioSource(string id, GameObject anchor, AudioResourceSO resource, UnityEngine.AudioSource source)
        : base(id, anchor, resource)
    {
        this.source = source;
        this.Duration = source?.clip?.length ?? 0.0f;
    }

    public override void Play()
    {
        this.source?.Play();
    }

    public override void Stop()
    {
        this.source?.Stop();
    }
}
