using System;
using UnityEngine;

[Serializable]
public class CameraShake
{
    public bool HasFinished => this.currentTime >= this.Duration;
    public Vector3 Displacement { get; protected set; }

    [SerializeField]
    [Range(0.01f, 2.0f)]
    private float magnitude = 0.15f;
    public float Magnitude { get => this.magnitude; set => this.magnitude = Mathf.Clamp01(value); }

    [SerializeField]
    [Range(0.1f, 10.0f)]
    private float duration = 1.0f;
    public float Duration { get => this.duration; set => this.duration = Mathf.Clamp(value, 0.0f, 10.0f); }

    private float currentTime;

    public virtual void Update()
    {
        this.currentTime += Time.deltaTime;
        this.Displacement = Mathf.Lerp(this.magnitude, 0.0f, this.currentTime / this.duration) * UnityEngine.Random.insideUnitSphere;
    }
}
