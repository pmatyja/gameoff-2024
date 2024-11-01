using UnityEngine;

public interface IAudioResource
{
    public float Duration { get; }

    public IAudioSource CreateAudioSource(string id, AudioResourceSO resource, GameObject anchor);
}