using System;
using UnityEngine;

[Serializable]
public struct ZoomCameraTargetEventParameters
{
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float zoom;
    public float Zoom { get => this.zoom; set => this.zoom = Mathf.Clamp01(value); }
}