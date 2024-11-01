using System;

[Serializable]
public class WeatherEffectEvent
{
    public string Id;
    public float Likelyhood = 0.1f;
    public SoundEffectSO SoundEffects;
    public CameraShake CameraShake;
}
