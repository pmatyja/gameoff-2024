using System;
using UnityEngine;

[Serializable]
public struct ShakeCameraTargetEventParameters
{
    [SerializeField]
    public bool WorldSpace;

    [SerializeField]
    public Vector3 Position;

    [SerializeField]
    [Range(0.01f, 1.0f)]
    private float magnitude;
    public float Magnitude { get => this.magnitude; set => this.magnitude = Mathf.Clamp01(value); }

    [SerializeField]
    [Range(0.1f, 10.0f)]
    private float duration;
    public float Duration { get => this.duration; set => this.duration = Mathf.Clamp(value, 0.0f, 10.0f); }
}