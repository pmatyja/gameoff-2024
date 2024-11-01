using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 360f)]
    private float halfAngleRange = 45.0f;

    [SerializeField]
    [Range(0f, 360f)]
    private float duration = 30.0f;

    protected float progress;
    private int loopCount = int.MaxValue;

    private Quaternion baseRotation;

    protected virtual void Awake()
    {
        this.baseRotation = this.transform.rotation;
    }

    protected virtual void Update()
    {
        this.progress = Tween.Interpolate(this.progress, this.duration, ref this.loopCount, true, onUpdate: t =>
        { 
            this.transform.rotation = Quaternion.AngleAxis(Mathf.Lerp(-this.halfAngleRange, this.halfAngleRange, Easing.SineInOut(t)), Vector3.up) * this.baseRotation;
        });
    }
}
