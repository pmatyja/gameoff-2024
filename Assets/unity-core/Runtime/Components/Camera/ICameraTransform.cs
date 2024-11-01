using UnityEngine;

public abstract class BaseCamera : MonoBehaviour
{
    public abstract Quaternion InputOffset { get; }
}
