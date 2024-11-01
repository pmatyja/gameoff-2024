using UnityEngine;

public interface ICamera
{
    GameObject MainTarget { get; set; }
    GameObject TemporaryTarget { get; set; }
    Vector3 Offset { get; set; }
    float Smoothness { get; set; }
}