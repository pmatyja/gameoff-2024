using System;
using UnityEngine;

[Serializable]
public class UnityAudioResource : IAudioResource
{
    [HideLabel]
    public AudioClip Clip;
    public float Duration => this.Clip?.length ?? 0.0f;

    public IAudioSource CreateAudioSource(string id, AudioResourceSO resource, GameObject anchor)
    {
        if (this.Clip == null)
        {
            return default;
        }

        var component = anchor.AddComponent<AudioSource>();

        component.clip = this.Clip;

        return new UnityAudioSource(id, anchor, resource, component);
    }
}
